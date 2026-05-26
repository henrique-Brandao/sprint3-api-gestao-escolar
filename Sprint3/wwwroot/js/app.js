const state = {
  token: localStorage.getItem('sprint3_token') || '',
  role: localStorage.getItem('sprint3_role') || '',
  email: localStorage.getItem('sprint3_email') || '',
  nome: localStorage.getItem('sprint3_nome') || '',
  alunoId: localStorage.getItem('sprint3_alunoId') || '',
  page: '',
  editingId: null,
};

const resources = {
  alunoPortal: {
    title: 'Portal do Aluno',
    subtitle: 'Suas disciplinas e notas cadastradas.',
    endpoint: '/api/Nota',
    roles: ['Aluno'],
    rolesWrite: [],
    columns: [
      { key: 'disciplinaNome', label: 'Disciplina' },
      { key: 'valor', label: 'Nota' },
      { key: 'alunoNome', label: 'Aluno' },
    ],
  },
  notas: {
    title: 'Notas',
    subtitle: 'Lançamento e acompanhamento de notas.',
    endpoint: '/api/Nota',
    roles: ['Professor', 'Diretor', 'Admin'],
    rolesWrite: ['Professor', 'Diretor', 'Admin'],
    fields: [
      { name: 'valor', label: 'Valor', type: 'number', min: 0, max: 10, step: 0.1, required: true },
      { name: 'alunoNome', label: 'Aluno', type: 'text', required: true },
      { name: 'disciplinaNome', label: 'Disciplina', type: 'text', required: true },
    ],
    columns: [
      { key: 'alunoNome', label: 'Aluno' },
      { key: 'disciplinaNome', label: 'Disciplina' },
      { key: 'valor', label: 'Nota' },
    ],
  },
  disciplinas: {
    title: 'Disciplinas',
    subtitle: 'Componentes curriculares e professores responsáveis.',
    endpoint: '/api/Disciplina',
    roles: ['Aluno', 'Professor', 'Diretor', 'Admin'],
    rolesWrite: ['Professor', 'Diretor', 'Admin'],
    fields: [
      { name: 'nome', label: 'Disciplina', type: 'text', required: true },
      { name: 'professorNome', label: 'Professor', type: 'text', required: true },
    ],
    columns: [
      { key: 'nome', label: 'Disciplina' },
      { key: 'professorNome', label: 'Professor' },
    ],
  },
  alunos: {
    title: 'Alunos',
    subtitle: 'Cadastro e consulta de estudantes.',
    endpoint: '/api/Aluno',
    roles: ['Professor', 'Diretor', 'Admin'],
    rolesWrite: ['Diretor', 'Admin'],
    fields: [
      { name: 'nome', label: 'Nome', type: 'text', required: true },
      { name: 'email', label: 'Email', type: 'email', required: true },
    ],
    columns: [
      { key: 'nome', label: 'Nome' },
      { key: 'email', label: 'Email' },
      { key: 'media', label: 'Média' },
      { key: 'situacao', label: 'Situação' },
    ],
  },
  professores: {
    title: 'Professores',
    subtitle: 'Equipe docente cadastrada.',
    endpoint: '/api/Professor',
    roles: ['Professor', 'Diretor', 'Admin'],
    rolesWrite: ['Diretor', 'Admin'],
    fields: [
      { name: 'nome', label: 'Nome', type: 'text', required: true },
      { name: 'email', label: 'Email', type: 'email', required: true },
    ],
    columns: [
      { key: 'nome', label: 'Nome' },
      { key: 'email', label: 'Email' },
      { key: 'disciplinas', label: 'Disciplinas', format: v => Array.isArray(v) ? v.join(', ') || '-' : '-' },
    ],
  },
  diretores: {
    title: 'Diretores',
    subtitle: 'Gestão da equipe de direção.',
    endpoint: '/api/Diretor',
    roles: ['Admin'],
    rolesWrite: ['Admin'],
    fields: [
      { name: 'nome', label: 'Nome', type: 'text', required: true },
      { name: 'email', label: 'Email', type: 'email', required: true },
    ],
    columns: [
      { key: 'nome', label: 'Nome' },
      { key: 'email', label: 'Email' },
    ],
  },
  usuarios: {
    title: 'Usuários',
    subtitle: 'Contas e perfis de acesso.',
    endpoint: '/api/Usuario',
    roles: ['Admin'],
    rolesWrite: ['Admin'],
    allowEdit: false,
    fields: [
      { name: 'nome', label: 'Nome', type: 'text', required: true },
      { name: 'email', label: 'Email', type: 'email', required: true },
      { name: 'senha', label: 'Senha inicial', type: 'password', required: true },
      { name: 'role', label: 'Perfil', type: 'select', options: ['Aluno', 'Professor', 'Diretor', 'Admin'], required: true },
      { name: 'alunoId', label: 'Aluno vinculado', type: 'select-entity', endpoint: '/api/Aluno' },
      { name: 'professorId', label: 'Professor vinculado', type: 'select-entity', endpoint: '/api/Professor' },
      { name: 'diretorId', label: 'Diretor vinculado', type: 'select-entity', endpoint: '/api/Diretor' },
    ],
    columns: [
      { key: 'nome', label: 'Nome' },
      { key: 'email', label: 'Email' },
      { key: 'role', label: 'Perfil' },
    ],
  },
  solicitacoes: {
    title: 'Solicitações de acesso',
    subtitle: 'Pedidos pendentes de alunos e professores.',
    endpoint: '/api/SolicitacaoAcesso',
    roles: ['Diretor', 'Admin'],
    rolesWrite: ['Diretor', 'Admin'],
    noEdit: true,
    noDelete: true,
    columns: [
      { key: 'nome', label: 'Nome' },
      { key: 'email', label: 'Email' },
      { key: 'tipoSolicitado', label: 'Tipo' },
      { key: 'status', label: 'Status' },
      { key: 'mensagem', label: 'Mensagem' },
    ],
  },
};

const roleHome = { Aluno: 'alunoPortal', Professor: 'disciplinas', Diretor: 'alunos', Admin: 'usuarios' };
const roleTitle = { Aluno: 'Portal do Aluno', Professor: 'Portal do Professor', Diretor: 'Portal da Direção', Admin: 'Painel Administrativo' };
const menuLabels = { alunoPortal: 'Meu boletim', disciplinas: 'Disciplinas', notas: 'Notas', alunos: 'Alunos', professores: 'Professores', diretores: 'Diretores', usuarios: 'Usuários', solicitacoes: 'Solicitações' };

const els = {
  authScreen: document.getElementById('authScreen'),
  portalScreen: document.getElementById('portalScreen'),
  loginCard: document.getElementById('loginCard'),
  accessCard: document.getElementById('accessCard'),
  authAlert: document.getElementById('authAlert'),
  loginForm: document.getElementById('loginForm'),
  accessForm: document.getElementById('accessForm'),
  emailInput: document.getElementById('emailInput'),
  senhaInput: document.getElementById('senhaInput'),
  menuArea: document.getElementById('menuArea'),
  roleLabel: document.getElementById('roleLabel'),
  portalTitle: document.getElementById('portalTitle'),
  userBadge: document.getElementById('userBadge'),
  logoutBtn: document.getElementById('logoutBtn'),
  alertArea: document.getElementById('alertArea'),
  summaryArea: document.getElementById('summaryArea'),
  pageTitle: document.getElementById('pageTitle'),
  pageSubtitle: document.getElementById('pageSubtitle'),
  reloadBtn: document.getElementById('reloadBtn'),
  newBtn: document.getElementById('newBtn'),
  tableHead: document.getElementById('tableHead'),
  tableBody: document.getElementById('tableBody'),
  modal: new bootstrap.Modal(document.getElementById('entityModal')),
  modalTitle: document.getElementById('modalTitle'),
  modalBody: document.getElementById('modalBody'),
  entityForm: document.getElementById('entityForm'),
};

function showAuth(mode) {
  els.loginCard.classList.toggle('d-none', mode !== 'login');
  els.accessCard.classList.toggle('d-none', mode !== 'access');
  els.authAlert.innerHTML = '';
}

function showAlert(message, type = 'info', target = els.alertArea) {
  target.innerHTML = `<div class="alert alert-${type}" role="alert">${escapeHtml(message)}</div>`;
}

async function apiFetch(url, options = {}) {
  const headers = { 'Content-Type': 'application/json', ...(options.headers || {}) };
  if (state.token) headers.Authorization = `Bearer ${state.token}`;
  const response = await fetch(url, { ...options, headers });
  if (response.status === 401) {
    clearAuth();
    throw new Error('401 - sessão expirada ou ausente.');
  }
  if (response.status === 204) return null;
  const contentType = response.headers.get('content-type') || '';
  const data = contentType.includes('application/json') ? await response.json() : await response.text();
  if (!response.ok) {
    const message = typeof data === 'string' ? data : (data.mensagem || data.title || 'Erro na requisição.');
    throw new Error(`${response.status} - ${message}`);
  }
  return data;
}

function saveAuth(data, email) {
  Object.assign(state, {
    token: data.token,
    role: data.role,
    email,
    nome: data.nome,
    alunoId: data.alunoId || '',
  });
  localStorage.setItem('sprint3_token', state.token);
  localStorage.setItem('sprint3_role', state.role);
  localStorage.setItem('sprint3_email', state.email);
  localStorage.setItem('sprint3_nome', state.nome);
  localStorage.setItem('sprint3_alunoId', state.alunoId);
}

function clearAuth() {
  ['sprint3_token', 'sprint3_role', 'sprint3_email', 'sprint3_nome', 'sprint3_alunoId'].forEach(k => localStorage.removeItem(k));
  Object.assign(state, { token: '', role: '', email: '', nome: '', alunoId: '', page: '' });
  renderShell();
}

async function login(event) {
  event.preventDefault();
  try {
    const data = await apiFetch('/api/Auth/login', {
      method: 'POST',
      body: JSON.stringify({ email: els.emailInput.value.trim(), senha: els.senhaInput.value }),
      headers: {},
    });
    saveAuth(data, els.emailInput.value.trim());
    state.page = roleHome[state.role];
    renderShell();
    await loadCurrentPage();
  } catch (error) {
    showAlert(`Falha no login: ${error.message}`, 'danger', els.authAlert);
  }
}

async function sendAccessRequest(event) {
  event.preventDefault();
  try {
    await apiFetch('/api/SolicitacaoAcesso', {
      method: 'POST',
      body: JSON.stringify({
        nome: document.getElementById('accessNome').value.trim(),
        email: document.getElementById('accessEmail').value.trim(),
        tipoSolicitado: document.getElementById('accessTipo').value,
        mensagem: document.getElementById('accessMensagem').value.trim(),
      }),
      headers: {},
    });
    els.accessForm.reset();
    showAlert('Solicitação enviada. Ela ficará pendente até aprovação.', 'success', els.authAlert);
  } catch (error) {
    showAlert(`Erro ao solicitar acesso: ${error.message}`, 'danger', els.authAlert);
  }
}

function allowedResources() {
  return Object.entries(resources).filter(([, r]) => r.roles.includes(state.role));
}

function renderShell() {
  const logged = Boolean(state.token);
  els.authScreen.classList.toggle('d-none', logged);
  els.portalScreen.classList.toggle('d-none', !logged);
  if (!logged) return;

  if (!state.page || !resources[state.page]?.roles.includes(state.role)) {
    state.page = roleHome[state.role];
  }

  els.portalTitle.textContent = roleTitle[state.role] || 'Portal Escolar';
  els.roleLabel.textContent = state.role;
  els.userBadge.textContent = `${state.nome || state.email}`;
  els.menuArea.innerHTML = allowedResources().map(([key]) => `
    <button class="menu-button ${state.page === key ? 'active' : ''}" data-page="${key}">${menuLabels[key]}</button>
  `).join('');
  document.querySelectorAll('[data-page]').forEach(button => button.addEventListener('click', () => setPage(button.dataset.page)));
  renderSummary();
}

function renderSummary() {
  const blocks = {
    Aluno: [['Perfil', 'Aluno'], ['Acesso', 'Somente visualização'], ['Área', 'Notas próprias']],
    Professor: [['Perfil', 'Professor'], ['Gestão', 'Notas e disciplinas'], ['Consulta', 'Alunos']],
    Diretor: [['Perfil', 'Direção'], ['Gestão', 'Alunos e professores'], ['Acessos', 'Solicitações']],
    Admin: [['Perfil', 'Admin'], ['Gestão', 'Todos os módulos'], ['Segurança', 'Usuários e perfis']],
  }[state.role] || [];
  els.summaryArea.innerHTML = blocks.map(([label, value]) => `<article><span>${label}</span><strong>${value}</strong></article>`).join('');
}

function setPage(page) {
  if (!resources[page]?.roles.includes(state.role)) {
    showAlert('Acesso negado para este portal.', 'warning');
    return;
  }
  state.page = page;
  renderShell();
  loadCurrentPage();
}

function canWrite(resource) {
  return resource.rolesWrite.includes(state.role);
}

async function loadCurrentPage() {
  const resource = resources[state.page];
  els.pageTitle.textContent = resource.title;
  els.pageSubtitle.textContent = resource.subtitle;
  els.newBtn.classList.toggle('d-none', !canWrite(resource) || resource.noEdit);
  try {
    const data = await apiFetch(resource.endpoint);
    renderTable(Array.isArray(data) ? data : [data]);
  } catch (error) {
    renderTable([]);
    showAlert(`Erro ao carregar dados: ${error.message}`, 'danger');
  }
}

function renderTable(items) {
  const resource = resources[state.page];
  const showActions = canWrite(resource);
  const columns = showActions ? [...resource.columns, { key: 'actions', label: 'Ações' }] : resource.columns;
  els.tableHead.innerHTML = `<tr>${columns.map(c => `<th>${c.label}</th>`).join('')}</tr>`;
  if (!items.length) {
    els.tableBody.innerHTML = `<tr><td colspan="${columns.length}" class="text-center text-muted py-4">Nenhum registro encontrado.</td></tr>`;
    return;
  }
  els.tableBody.innerHTML = items.map(item => {
    const cells = resource.columns.map(column => {
      const raw = item[column.key];
      const value = column.format ? column.format(raw, item) : (raw ?? '-');
      return `<td>${escapeHtml(String(value))}</td>`;
    }).join('');
    const actions = showActions ? renderActions(item, resource) : '';
    return `<tr>${cells}${actions}</tr>`;
  }).join('');
}

function renderActions(item, resource) {
  if (state.page === 'solicitacoes') {
    const disabled = item.status !== 'Pendente' ? 'disabled' : '';
    return `<td class="action-cell">
      <button class="btn btn-sm btn-success me-1" ${disabled} onclick="approveRequest(${item.id})">Aprovar</button>
      <button class="btn btn-sm btn-outline-danger" ${disabled} onclick="rejectRequest(${item.id})">Recusar</button>
    </td>`;
  }
  const editButton = resource.allowEdit === false
    ? ''
    : `<button class="btn btn-sm btn-outline-primary me-1" onclick="openEditModal(${item.id})">Editar</button>`;

  return `<td class="action-cell">
    ${editButton}
    <button class="btn btn-sm btn-outline-danger" onclick="deleteItem(${item.id})">Excluir</button>
  </td>`;
}

async function openCreateModal() {
  state.editingId = null;
  const resource = resources[state.page];
  els.modalTitle.textContent = `Novo - ${resource.title}`;
  await renderForm(resource.fields || [], {});
  els.modal.show();
}

async function openEditModal(id) {
  const resource = resources[state.page];
  const item = await apiFetch(`${resource.endpoint}/${id}`);
  state.editingId = id;
  els.modalTitle.textContent = `Editar - ${resource.title}`;
  await renderForm(resource.fields || [], item);
  els.modal.show();
}

async function renderForm(fields, item) {
  const preparedFields = await Promise.all(fields.map(async field => {
    if (field.type !== 'select-entity') return field;
    const data = await apiFetch(field.endpoint);
    const options = Array.isArray(data) ? data.map(option => ({
      value: option.id,
      label: option.nome || option.email || `Registro ${option.id}`,
    })) : [];
    return { ...field, options };
  }));

  els.modalBody.innerHTML = preparedFields.map(field => {
    const value = item[field.name] ?? '';
    if (field.type === 'select') {
      return `<div class="mb-3"><label class="form-label">${field.label}</label><select class="form-select" name="${field.name}">${field.options.map(o => `<option value="${o}" ${o === value ? 'selected' : ''}>${o}</option>`).join('')}</select></div>`;
    }
    if (field.type === 'select-entity') {
      return `<div class="mb-3">
        <label class="form-label">${field.label}</label>
        <select class="form-select" name="${field.name}">
          <option value="">Nenhum</option>
          ${field.options.map(o => `<option value="${o.value}" ${String(o.value) === String(value) ? 'selected' : ''}>${escapeHtml(o.label)}</option>`).join('')}
        </select>
      </div>`;
    }
    return `<div class="mb-3">
      <label class="form-label">${field.label}</label>
      <input class="form-control" name="${field.name}" type="${field.type}" value="${escapeHtml(String(value))}" ${field.required ? 'required' : ''} ${field.min !== undefined ? `min="${field.min}"` : ''} ${field.max !== undefined ? `max="${field.max}"` : ''} ${field.step ? `step="${field.step}"` : ''}>
    </div>`;
  }).join('');
}

function formPayload() {
  const resource = resources[state.page];
  const data = new FormData(els.entityForm);
  const payload = {};
  (resource.fields || []).forEach(field => {
    const raw = data.get(field.name);
    if (field.type === 'number' || field.type === 'select-entity') payload[field.name] = raw === '' ? null : Number(raw);
    else payload[field.name] = String(raw || '').trim();
  });
  return payload;
}

async function submitForm(event) {
  event.preventDefault();
  const resource = resources[state.page];
  const url = state.editingId ? `${resource.endpoint}/${state.editingId}` : resource.endpoint;
  const method = state.editingId ? 'PUT' : 'POST';
  try {
    await apiFetch(url, { method, body: JSON.stringify(formPayload()) });
    els.modal.hide();
    showAlert('Registro salvo com sucesso.', 'success');
    await loadCurrentPage();
  } catch (error) {
    showAlert(`Erro ao salvar: ${error.message}`, 'danger');
  }
}

async function deleteItem(id) {
  if (!confirm('Deseja excluir este registro?')) return;
  try {
    await apiFetch(`${resources[state.page].endpoint}/${id}`, { method: 'DELETE' });
    showAlert('Registro removido.', 'success');
    await loadCurrentPage();
  } catch (error) {
    showAlert(`Erro ao excluir: ${error.message}`, 'danger');
  }
}

async function approveRequest(id) {
  const senhaInicial = prompt('Senha inicial para o usuário aprovado:', 'Temp@123');
  if (senhaInicial === null) return;
  if (senhaInicial.trim() !== '' && senhaInicial.length < 6) {
    showAlert('A senha inicial precisa ter pelo menos 6 caracteres. Deixe em branco para usar Temp@123.', 'warning');
    return;
  }
  try {
    const result = await apiFetch(`/api/SolicitacaoAcesso/${id}/aprovar`, { method: 'POST', body: JSON.stringify({ senhaInicial }) });
    showAlert(`Solicitação aprovada. Senha temporária: ${result.senhaTemporaria}`, 'success');
    await loadCurrentPage();
  } catch (error) {
    showAlert(`Erro ao aprovar: ${error.message}`, 'danger');
  }
}

async function rejectRequest(id) {
  try {
    await apiFetch(`/api/SolicitacaoAcesso/${id}/recusar`, { method: 'POST' });
    showAlert('Solicitação recusada.', 'success');
    await loadCurrentPage();
  } catch (error) {
    showAlert(`Erro ao recusar: ${error.message}`, 'danger');
  }
}

function escapeHtml(value) {
  return value.replaceAll('&', '&amp;').replaceAll('<', '&lt;').replaceAll('>', '&gt;').replaceAll('"', '&quot;').replaceAll("'", '&#039;');
}

window.openEditModal = openEditModal;
window.deleteItem = deleteItem;
window.approveRequest = approveRequest;
window.rejectRequest = rejectRequest;

els.loginForm.addEventListener('submit', login);
els.accessForm.addEventListener('submit', sendAccessRequest);
els.logoutBtn.addEventListener('click', clearAuth);
els.reloadBtn.addEventListener('click', loadCurrentPage);
els.newBtn.addEventListener('click', openCreateModal);
els.entityForm.addEventListener('submit', submitForm);
document.getElementById('showAccessRequestBtn').addEventListener('click', () => showAuth('access'));
document.getElementById('showLoginBtn').addEventListener('click', () => showAuth('login'));
document.querySelectorAll('.preset-login').forEach(btn => btn.addEventListener('click', () => {
  els.emailInput.value = btn.dataset.email;
  els.senhaInput.value = '123456';
}));

renderShell();
if (state.token) loadCurrentPage();
