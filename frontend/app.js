const state = {
  user: null,
  autores: [],
  livros: [],
  alunos: [],
  emprestimos: [],
  lastFocusedElement: null
};

const pageMeta = {
  dashboard: ['Visão geral', 'Dashboard'],
  livros: ['Acervo', 'Livros'],
  emprestimos: ['Circulação', 'Empréstimos']
};

const $ = selector => document.querySelector(selector);
const $$ = selector => [...document.querySelectorAll(selector)];

function escapeHtml(value) {
  return String(value ?? '')
    .replaceAll('&', '&amp;')
    .replaceAll('<', '&lt;')
    .replaceAll('>', '&gt;')
    .replaceAll('"', '&quot;')
    .replaceAll("'", '&#039;');
}

function formatDate(value) {
  if (!value) return '—';
  const date = new Date(value);
  return Number.isNaN(date.getTime()) ? '—' : date.toLocaleDateString('pt-BR');
}

async function api(path, options = {}) {
  const { acceptError = false, ...fetchOptions } = options;
  const response = await fetch(path, {
    ...fetchOptions,
    headers: {
      'Content-Type': 'application/json',
      ...(fetchOptions.headers || {})
    }
  });

  let payload = null;
  const text = await response.text();
  if (text) {
    try { payload = JSON.parse(text); } catch { payload = text; }
  }

  if (!response.ok && !acceptError) {
    const message = payload?.detail || payload?.message || payload?.title || (typeof payload === 'string' ? payload : `HTTP ${response.status}`);
    const error = new Error(message);
    error.status = response.status;
    error.payload = payload;
    throw error;
  }

  return payload;
}

function setLoading(button, loading, loadingText = 'Aguarde...') {
  if (!button) return;
  if (loading) {
    button.dataset.originalText = button.innerHTML;
    button.disabled = true;
    button.textContent = loadingText;
  } else {
    button.disabled = false;
    if (button.dataset.originalText) button.innerHTML = button.dataset.originalText;
  }
}

function toast(message, type = 'success') {
  const el = document.createElement('div');
  el.className = `toast ${type}`;
  el.textContent = message;
  $('#toast-region').append(el);
  setTimeout(() => el.remove(), 3600);
}

function animateNumber(element, value) {
  const target = Number(value) || 0;
  const start = Number(element.textContent) || 0;
  if (window.matchMedia('(prefers-reduced-motion: reduce)').matches || start === target) {
    element.textContent = target;
    return;
  }

  const startedAt = performance.now();
  const duration = 520;
  const tick = now => {
    const progress = Math.min((now - startedAt) / duration, 1);
    const eased = 1 - Math.pow(1 - progress, 3);
    element.textContent = Math.round(start + (target - start) * eased);
    if (progress < 1) requestAnimationFrame(tick);
  };
  requestAnimationFrame(tick);
}

function showInlineError(selector, message) {
  const el = $(selector);
  el.textContent = message;
  el.hidden = false;
}

function clearInlineError(selector) {
  const el = $(selector);
  el.hidden = true;
  el.textContent = '';
}

function initials(name) {
  return (name || 'B').split(/\s+/).slice(0, 2).map(part => part[0]).join('').toUpperCase();
}

function showApp(user) {
  state.user = user;
  sessionStorage.setItem('bibliotecaUser', JSON.stringify(user));
  $('#login-view').hidden = true;
  $('#app-view').hidden = false;
  $('#user-name').textContent = user.name || 'Bibliotecário(a)';
  $('#user-avatar').textContent = initials(user.name);
  navigate('dashboard');
  loadEverything();
}

function logout() {
  sessionStorage.removeItem('bibliotecaUser');
  state.user = null;
  $('#app-view').hidden = true;
  $('#login-view').hidden = false;
  $('#login-password').focus();
}

async function handleLogin(event) {
  event.preventDefault();
  clearInlineError('#login-error');
  const button = $('#login-submit');
  setLoading(button, true, 'Entrando...');

  try {
    const result = await api('/api/auth/login', {
      method: 'POST',
      body: JSON.stringify({
        email: $('#login-email').value.trim(),
        senha: $('#login-password').value
      })
    });
    showApp(result.user);
  } catch (error) {
    showInlineError('#login-error', error.message);
  } finally {
    setLoading(button, false);
  }
}

function navigate(page) {
  $$('.page').forEach(el => el.classList.toggle('active', el.dataset.pagePanel === page));
  $$('.nav-item[data-page]').forEach(el => el.classList.toggle('active', el.dataset.page === page));
  const [kicker, title] = pageMeta[page] || pageMeta.dashboard;
  $('#page-kicker').textContent = kicker;
  $('#page-title').textContent = title;
  $('#app-view').classList.remove('sidebar-open');

  if (page === 'livros') renderBooks();
  if (page === 'emprestimos') renderLoans();
}

async function loadEverything() {
  const results = await Promise.all([loadCoreData(), loadHealth()]);
  return results.every(Boolean);
}

async function loadCoreData() {
  try {
    const [autores, livros, alunos, emprestimos] = await Promise.all([
      api('/api/autores'),
      api('/api/livros'),
      api('/api/alunos'),
      api('/api/emprestimos')
    ]);

    state.autores = autores || [];
    state.livros = livros || [];
    state.alunos = alunos || [];
    state.emprestimos = emprestimos || [];

    renderDashboard();
    renderBooks();
    renderLoans();
    fillSelects();
    return true;
  } catch (error) {
    toast(`Não foi possível carregar os dados: ${error.message}`, 'error');
    return false;
  }
}

async function loadHealth() {
  const ids = ['#health-api', '#health-db', '#health-redis'];
  ids.forEach(id => setHealth($(id), 'checking'));

  try {
    const health = await api('/api/health', { acceptError: true });
    if (!health || typeof health !== 'object') throw new Error('Resposta de saúde inválida.');
    setHealth($('#health-api'), health.api === 'running' ? 'ok' : 'error');
    setHealth($('#health-db'), health.database === 'running' ? 'ok' : 'error');
    setHealth($('#health-redis'), health.redis === 'running' ? 'ok' : 'error');

    const allOk = health.api === 'running' && health.database === 'running' && health.redis === 'running';
    $('#sidebar-status-dot').className = `status-dot ${allOk ? 'ok' : 'error'}`;
    $('#sidebar-status-text').textContent = allOk ? 'Serviços online' : 'Verificar serviços';
    return allOk;
  } catch {
    ids.forEach(id => setHealth($(id), 'error'));
    $('#sidebar-status-dot').className = 'status-dot error';
    $('#sidebar-status-text').textContent = 'API indisponível';
    return false;
  }
}

function setHealth(el, status) {
  const labels = { checking: 'checando', ok: 'running', error: 'falha' };
  el.className = `health-pill ${status}`;
  el.textContent = labels[status];
}

function renderDashboard() {
  const active = state.emprestimos.filter(e => e.status === 'Ativo').length;
  const available = state.livros.reduce((sum, livro) => sum + Number(livro.quantidade || 0), 0);

  animateNumber($('#count-livros'), state.livros.length);
  $('#count-exemplares').textContent = `${available} exemplares disponíveis`;
  animateNumber($('#count-emprestimos'), state.emprestimos.length);
  $('#count-ativos').textContent = `${active} ativos`;
  animateNumber($('#count-alunos'), state.alunos.length);
  animateNumber($('#count-autores'), state.autores.length);

  const rows = [...state.emprestimos].sort((a, b) => b.id - a.id).slice(0, 5);
  $('#recent-loans-body').innerHTML = rows.length ? rows.map(e => `
    <tr>
      <td>${escapeHtml(e.nomeAluno)}</td>
      <td>${escapeHtml(e.tituloLivro)}</td>
      <td>${statusBadge(e.status)}</td>
      <td>${formatDate(e.dataPrevistaDevolucao)}</td>
    </tr>`).join('') : emptyRow(4, 'Nenhum empréstimo registrado ainda.');
}

function renderBooks() {
  const query = ($('#book-search')?.value || '').trim().toLowerCase();
  const rows = state.livros.filter(livro => {
    const haystack = `${livro.titulo} ${livro.nomeAutor} ${livro.isbn}`.toLowerCase();
    return haystack.includes(query);
  });

  $('#book-result-count').textContent = `${rows.length} ${rows.length === 1 ? 'livro' : 'livros'}`;
  $('#books-body').innerHTML = rows.length ? rows.map(livro => `
    <tr>
      <td>#${livro.id}</td>
      <td class="book-cell"><strong>${escapeHtml(livro.titulo)}</strong><small>${escapeHtml(livro.nomeAutor)}</small></td>
      <td>${escapeHtml(livro.isbn)}</td>
      <td>${livro.anoPublicacao || '—'}</td>
      <td><span class="badge ${livro.quantidade > 0 ? 'success' : 'warning'}">${livro.quantidade} ${livro.quantidade === 1 ? 'exemplar' : 'exemplares'}</span></td>
    </tr>`).join('') : emptyRow(5, 'Nenhum livro encontrado.');
}

function renderLoans() {
  const query = ($('#loan-search')?.value || '').trim().toLowerCase();
  const status = $('#loan-status-filter')?.value || '';
  const rows = [...state.emprestimos]
    .sort((a, b) => b.id - a.id)
    .filter(e => `${e.nomeAluno} ${e.tituloLivro}`.toLowerCase().includes(query))
    .filter(e => !status || e.status === status);

  $('#loan-total').textContent = state.emprestimos.length;
  $('#loan-active').textContent = state.emprestimos.filter(e => e.status === 'Ativo').length;
  $('#loan-returned').textContent = state.emprestimos.filter(e => e.status === 'Devolvido').length;

  $('#loans-body').innerHTML = rows.length ? rows.map(e => `
    <tr>
      <td>#${e.id}</td>
      <td>${escapeHtml(e.nomeAluno)}</td>
      <td>${escapeHtml(e.tituloLivro)}</td>
      <td>${formatDate(e.dataEmprestimo)}</td>
      <td>${formatDate(e.dataPrevistaDevolucao)}</td>
      <td>${statusBadge(e.status)}</td>
      <td>${e.status === 'Ativo' ? `<button class="action-small return-loan" data-loan-id="${e.id}" type="button">Registrar devolução</button>` : ''}</td>
    </tr>`).join('') : emptyRow(7, 'Nenhum empréstimo encontrado.');

  $$('.return-loan').forEach(button => button.addEventListener('click', () => returnLoan(button)));
}

function statusBadge(status) {
  if (status === 'Ativo') return '<span class="badge success">Ativo</span>';
  if (status === 'Devolvido') return '<span class="badge neutral">Devolvido</span>';
  if (status === 'Atrasado') return '<span class="badge warning">Atrasado</span>';
  return `<span class="badge warning">${escapeHtml(status)}</span>`;
}

function emptyRow(columns, message) {
  return `<tr><td colspan="${columns}" class="empty-cell">${escapeHtml(message)}</td></tr>`;
}

function fillSelects() {
  const author = $('#book-author');
  author.innerHTML = state.autores.length
    ? state.autores.map(a => `<option value="${a.id}">${escapeHtml(a.nome)}</option>`).join('')
    : '<option value="">Cadastre um autor primeiro</option>';

  const student = $('#loan-student');
  student.innerHTML = state.alunos.length
    ? state.alunos.map(a => `<option value="${a.id}">${escapeHtml(a.nome)} · ${escapeHtml(a.matricula)}</option>`).join('')
    : '<option value="">Cadastre um aluno primeiro</option>';

  const book = $('#loan-book');
  const available = state.livros.filter(l => Number(l.quantidade) > 0);
  book.innerHTML = available.length
    ? available.map(l => `<option value="${l.id}">${escapeHtml(l.titulo)} · ${l.quantidade} disp.</option>`).join('')
    : '<option value="">Nenhum livro disponível</option>';

  $('#save-book').disabled = state.autores.length === 0;
  $('#save-loan').disabled = state.alunos.length === 0 || available.length === 0;
}

function openModal(id) {
  const modal = $(`#${id}`);
  state.lastFocusedElement = document.activeElement;
  modal.hidden = false;
  document.body.style.overflow = 'hidden';
  requestAnimationFrame(() => modal.querySelector('input:not([type="hidden"]), select, button')?.focus());
}

function closeModal(id) {
  $(`#${id}`).hidden = true;
  document.body.style.overflow = '';
  state.lastFocusedElement?.focus();
}

async function createBook(event) {
  event.preventDefault();
  clearInlineError('#book-form-error');
  const button = $('#save-book');
  const form = new FormData(event.currentTarget);
  setLoading(button, true, 'Salvando...');

  try {
    await api('/api/livros', {
      method: 'POST',
      body: JSON.stringify({
        isbn: form.get('isbn')?.trim(),
        titulo: form.get('titulo')?.trim(),
        anoPublicacao: Number(form.get('anoPublicacao')),
        quantidade: Number(form.get('quantidade')),
        autorId: Number(form.get('autorId'))
      })
    });
    event.currentTarget.reset();
    event.currentTarget.querySelector('[name="anoPublicacao"]').value = String(new Date().getFullYear());
    event.currentTarget.querySelector('[name="quantidade"]').value = '1';
    closeModal('book-modal');
    await loadCoreData();
    toast('Livro cadastrado com sucesso.');
  } catch (error) {
    showInlineError('#book-form-error', error.message);
  } finally {
    setLoading(button, false);
  }
}

async function createLoan(event) {
  event.preventDefault();
  clearInlineError('#loan-form-error');
  const button = $('#save-loan');
  const form = new FormData(event.currentTarget);
  setLoading(button, true, 'Registrando...');

  try {
    const due = form.get('dataPrevistaDevolucao');
    if (!due) throw new Error('Informe a data prevista para devolução.');
    await api('/api/emprestimos', {
      method: 'POST',
      body: JSON.stringify({
        alunoId: Number(form.get('alunoId')),
        livroId: Number(form.get('livroId')),
        dataPrevistaDevolucao: new Date(`${due}T12:00:00Z`).toISOString()
      })
    });
    closeModal('loan-modal');
    await loadCoreData();
    toast('Empréstimo registrado com sucesso.');
  } catch (error) {
    showInlineError('#loan-form-error', error.message);
  } finally {
    setLoading(button, false);
  }
}

async function returnLoan(button) {
  const id = button.dataset.loanId;
  setLoading(button, true, 'Salvando...');
  try {
    await api(`/api/emprestimos/${id}/devolucao`, { method: 'PUT' });
    await loadCoreData();
    toast('Devolução registrada. Estoque atualizado.');
  } catch (error) {
    toast(error.message, 'error');
    setLoading(button, false);
  }
}

async function createAuthor() {
  const button = $('#create-author');
  const nome = $('#author-name').value.trim();
  const nacionalidade = $('#author-nationality').value.trim();
  const nascimento = $('#author-birth').value;
  if (!nome || !nascimento) return toast('Preencha nome e data de nascimento do autor.', 'error');
  setLoading(button, true, 'Salvando...');

  try {
    const result = await api('/api/autores', {
      method: 'POST',
      body: JSON.stringify({
        nome,
        nacionalidade,
        dataNascimento: new Date(`${nascimento}T12:00:00Z`).toISOString()
      })
    });
    state.autores.push(result);
    fillSelects();
    $('#book-author').value = String(result.id);
    $('#author-inline-fields').hidden = true;
    toast('Autor cadastrado e selecionado.');
  } catch (error) {
    toast(error.message, 'error');
  } finally {
    setLoading(button, false);
  }
}

async function createStudent() {
  const button = $('#create-student');
  const nome = $('#student-name').value.trim();
  const matricula = $('#student-registration').value.trim();
  const email = $('#student-email').value.trim();
  if (!nome || !matricula || !email) return toast('Preencha nome, matrícula e e-mail do aluno.', 'error');
  setLoading(button, true, 'Salvando...');

  try {
    const result = await api('/api/alunos', {
      method: 'POST',
      body: JSON.stringify({ nome, matricula, email })
    });
    state.alunos.push(result);
    fillSelects();
    $('#loan-student').value = String(result.id);
    $('#student-inline-fields').hidden = true;
    toast('Aluno cadastrado e selecionado.');
  } catch (error) {
    toast(error.message, 'error');
  } finally {
    setLoading(button, false);
  }
}

function setDefaultDueDate() {
  const date = new Date();
  date.setDate(date.getDate() + 7);
  const localDate = new Date(date.getTime() - date.getTimezoneOffset() * 60000);
  $('#loan-due-date').value = localDate.toISOString().slice(0, 10);
  $('#loan-due-date').min = new Date(Date.now() - new Date().getTimezoneOffset() * 60000).toISOString().slice(0, 10);
  $('#book-year').value = String(new Date().getFullYear());
}

function setCurrentDate() {
  const date = new Intl.DateTimeFormat('pt-BR', {
    weekday: 'long',
    day: '2-digit',
    month: 'long'
  }).format(new Date());
  $('#current-date').textContent = date.charAt(0).toUpperCase() + date.slice(1);
}

function bindEvents() {
  $('#login-form').addEventListener('submit', handleLogin);
  $('#toggle-password').addEventListener('click', () => {
    const input = $('#login-password');
    const willShow = input.type === 'password';
    input.type = willShow ? 'text' : 'password';
    $('#toggle-password').setAttribute('aria-pressed', String(willShow));
    $('#toggle-password').setAttribute('aria-label', willShow ? 'Ocultar senha' : 'Mostrar senha');
  });
  $('#logout-button').addEventListener('click', logout);

  $$('.nav-item[data-page]').forEach(button => button.addEventListener('click', () => navigate(button.dataset.page)));
  $$('[data-go]').forEach(button => button.addEventListener('click', () => navigate(button.dataset.go)));
  $('#menu-button').addEventListener('click', () => $('#app-view').classList.add('sidebar-open'));
  $('#sidebar-backdrop').addEventListener('click', () => $('#app-view').classList.remove('sidebar-open'));

  $('#refresh-dashboard').addEventListener('click', async event => {
    setLoading(event.currentTarget, true, 'Atualizando...');
    const success = await loadEverything();
    setLoading(event.currentTarget, false);
    toast(success ? 'Dados atualizados.' : 'Alguns serviços não responderam.', success ? 'success' : 'error');
  });

  $('#book-search').addEventListener('input', renderBooks);
  $('#loan-search').addEventListener('input', renderLoans);
  $('#loan-status-filter').addEventListener('change', renderLoans);

  $('#open-book-modal').addEventListener('click', () => { fillSelects(); openModal('book-modal'); });
  $('#open-loan-modal').addEventListener('click', () => { fillSelects(); setDefaultDueDate(); openModal('loan-modal'); });
  $$('[data-close-modal]').forEach(el => el.addEventListener('click', () => closeModal(el.dataset.closeModal)));
  $('#book-form').addEventListener('submit', createBook);
  $('#loan-form').addEventListener('submit', createLoan);

  $('#show-author-fields').addEventListener('click', () => $('#author-inline-fields').hidden = !$('#author-inline-fields').hidden);
  $('#show-student-fields').addEventListener('click', () => $('#student-inline-fields').hidden = !$('#student-inline-fields').hidden);
  $('#create-author').addEventListener('click', createAuthor);
  $('#create-student').addEventListener('click', createStudent);

  document.addEventListener('keydown', event => {
    if (event.key === 'Escape') {
      $$('.modal:not([hidden])').forEach(modal => closeModal(modal.id));
      $('#app-view').classList.remove('sidebar-open');
      return;
    }

    if (event.key === 'Tab') {
      const modal = $('.modal:not([hidden])');
      if (!modal) return;
      const focusable = $$(`#${modal.id} button:not(:disabled), #${modal.id} input:not(:disabled), #${modal.id} select:not(:disabled)`);
      if (!focusable.length) return;
      const first = focusable[0];
      const last = focusable.at(-1);
      if (event.shiftKey && document.activeElement === first) { event.preventDefault(); last.focus(); }
      if (!event.shiftKey && document.activeElement === last) { event.preventDefault(); first.focus(); }
    }
  });
}

function restoreSession() {
  try {
    const stored = sessionStorage.getItem('bibliotecaUser');
    if (stored) showApp(JSON.parse(stored));
  } catch {
    sessionStorage.removeItem('bibliotecaUser');
  }
}

bindEvents();
setDefaultDueDate();
setCurrentDate();
restoreSession();
