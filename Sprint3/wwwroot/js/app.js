const state = {
  token: localStorage.getItem('sprint3_token') || '',
  role: localStorage.getItem('sprint3_role') || '',
  email: localStorage.getItem('sprint3_email') || '',
  nome: localStorage.getItem('sprint3_nome') || '',
  alunoId: localStorage.getItem('sprint3_alunoId') || '',
  page: '',
  editingId: null,
  pageLoadId: 0,
};

const roleDisplay = {
  Admin: 'Administrador do Sistema',
  Diretor: 'Diretor',
  Professor: 'Professor',
  Aluno: 'Aluno',
};

const resources = {
  inicio: {
    title: 'Início',
    subtitle: 'Resumo das principais ações do portal.',
    roles: ['Aluno', 'Professor', 'Diretor', 'Admin'],
    rolesWrite: [],
    dashboard: true,
  },
  alunoPortal: {
    title: 'Minhas disciplinas',
    subtitle: 'Disciplinas em que você está matriculado e professores responsáveis.',
    endpoint: '/api/Matricula',
    roles: ['Aluno'],
    rolesWrite: [],
    columns: [
      { key: 'disciplinaNome', label: 'Disciplina' },
      { key: 'professorNome', label: 'Professor' },
    ],
  },
  boletim: {
    title: 'Boletim',
    subtitle: 'Notas lançadas e situação acadêmica por disciplina.',
    endpoint: '/api/Matricula',
    roles: ['Aluno'],
    rolesWrite: [],
    columns: [
      { key: 'disciplinaNome', label: 'Disciplina' },
      { key: 'nota', label: 'Nota', format: v => v ?? '-' },
      { key: 'status', label: 'Status', format: (_, item) => statusBoletim(item.nota) },
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
      { name: 'disciplinaId', label: 'Disciplina', type: 'select-entity', endpoint: '/api/Disciplina', required: true },
      { name: 'alunoId', label: 'Aluno matriculado', type: 'select-enrolled', endpoint: '/api/Matricula', required: true },
    ],
    columns: [
      { key: 'alunoNome', label: 'Aluno' },
      { key: 'disciplinaNome', label: 'Disciplina' },
      { key: 'valor', label: 'Nota' },
    ],
  },
  matriculas: {
    title: 'Matrículas',
    subtitle: 'Vínculo de alunos com disciplinas.',
    endpoint: '/api/Matricula',
    roles: ['Professor', 'Diretor', 'Admin'],
    rolesWrite: ['Diretor', 'Admin'],
    fields: [
      { name: 'alunoId', label: 'Aluno', type: 'select-entity', endpoint: '/api/Aluno', required: true },
      { name: 'disciplinaId', label: 'Disciplina', type: 'select-entity', endpoint: '/api/Disciplina', required: true },
      { name: 'status', label: 'Status', type: 'select', options: ['Ativa', 'Trancada', 'Concluída'], required: true },
    ],
    columns: [
      { key: 'alunoNome', label: 'Aluno' },
      { key: 'disciplinaNome', label: 'Disciplina' },
      { key: 'professorNome', label: 'Professor' },
      { key: 'status', label: 'Status da matrícula' },
      { key: 'nota', label: 'Nota', format: v => v ?? '-' },
      { key: 'situacao', label: 'Situação', format: (_, item) => statusBoletim(item.nota) },
    ],
  },
  disciplinas: {
    title: 'Disciplinas',
    subtitle: 'Componentes curriculares e professores responsáveis.',
    endpoint: '/api/Disciplina',
    roles: ['Diretor', 'Admin'],
    rolesWrite: ['Diretor', 'Admin'],
    fields: [
      { name: 'nome', label: 'Disciplina', type: 'text', required: true },
      { name: 'professorId', label: 'Professor', type: 'select-entity', endpoint: '/api/Professor', required: true },
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
    ],
  },
  professores: {
    title: 'Professores',
    subtitle: 'Equipe docente cadastrada.',
    endpoint: '/api/Professor',
    roles: ['Diretor', 'Admin'],
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
      { name: 'role', label: 'Perfil', type: 'select', options: [
        { value: 'Aluno', label: 'Aluno' },
        { value: 'Professor', label: 'Professor' },
        { value: 'Diretor', label: 'Diretor' },
        { value: 'Admin', label: 'Administrador do Sistema' },
      ], required: true },
      { name: 'alunoId', label: 'Aluno vinculado', type: 'select-entity', endpoint: '/api/Aluno' },
      { name: 'professorId', label: 'Professor vinculado', type: 'select-entity', endpoint: '/api/Professor' },
      { name: 'diretorId', label: 'Diretor vinculado', type: 'select-entity', endpoint: '/api/Diretor' },
    ],
    columns: [
      { key: 'nome', label: 'Nome' },
      { key: 'email', label: 'Email' },
      { key: 'role', label: 'Perfil', format: v => roleDisplay[v] || v },
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

const roleHome = { Aluno: 'inicio', Professor: 'inicio', Diretor: 'inicio', Admin: 'inicio' };
const roleTitle = { Aluno: 'Portal do Aluno', Professor: 'Portal do Professor', Diretor: 'Portal da Direção', Admin: 'Painel Administrativo' };
const menuLabels = { inicio: 'Início', alunoPortal: 'Minhas disciplinas', boletim: 'Boletim', disciplinas: 'Disciplinas', matriculas: 'Matrículas', notas: 'Notas', alunos: 'Alunos', professores: 'Professores', diretores: 'Diretores', usuarios: 'Usuários', solicitacoes: 'Solicitações' };

function statusBoletim(nota) {
  if (nota === null || nota === undefined || nota === '') return 'Sem nota';
  return Number(nota) >= 7 ? 'Aprovado' : 'Reprovado';
}

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
  newBtn: document.getElementById('newBtn'),
  dashboardArea: document.getElementById('dashboardArea'),
  tableHead: document.getElementById('tableHead'),
  tableBody: document.getElementById('tableBody'),
  modal: new bootstrap.Modal(document.getElementById('entityModal')),
  modalTitle: document.getElementById('modalTitle'),
  modalBody: document.getElementById('modalBody'),
  entityForm: document.getElementById('entityForm'),
  approvalModal: new bootstrap.Modal(document.getElementById('approvalModal')),
  approvalForm: document.getElementById('approvalForm'),
  approvalRequestId: document.getElementById('approvalRequestId'),
  approvalPassword: document.getElementById('approvalPassword'),
  approvalPasswordConfirm: document.getElementById('approvalPasswordConfirm'),
  approvalAlert: document.getElementById('approvalAlert'),
  requestDetailsModal: new bootstrap.Modal(document.getElementById('requestDetailsModal')),
  requestDetailsBody: document.getElementById('requestDetailsBody'),
  requestApproveBtn: document.getElementById('requestApproveBtn'),
  requestRejectBtn: document.getElementById('requestRejectBtn'),
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
  els.roleLabel.textContent = roleDisplay[state.role] || state.role;
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
    Admin: [['Perfil', 'Administrador do Sistema'], ['Gestão', 'Todos os módulos'], ['Segurança', 'Usuários e perfis']],
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
  return loadCurrentPage();
}

function canWrite(resource) {
  return resource.rolesWrite.includes(state.role);
}

function currentResource(page = state.page) {
  const resource = { ...resources[page], page };

  if (state.role === 'Professor' && page === 'alunos') {
    return {
      ...resource,
      subtitle: 'Alunos matriculados nas suas disciplinas.',
      endpoint: '/api/Matricula',
      columns: [
        { key: 'alunoNome', label: 'Aluno' },
        { key: 'disciplinaNome', label: 'Disciplina' },
        { key: 'nota', label: 'Nota', format: v => v ?? '-' },
      ],
    };
  }

  return resource;
}

function renderPageChrome(resource) {
  const hideNewButton = !canWrite(resource) || resource.noEdit;
  els.pageTitle.textContent = resource.title;
  els.pageSubtitle.textContent = resource.subtitle;
  els.newBtn.hidden = hideNewButton;
  els.newBtn.classList.toggle('d-none', hideNewButton);
  els.dashboardArea.classList.toggle('d-none', !resource.dashboard);
  document.querySelector('.table-responsive').classList.toggle('d-none', Boolean(resource.dashboard));
}

async function loadCurrentPage() {
  const page = state.page;
  const loadId = ++state.pageLoadId;
  const resource = currentResource(page);
  renderPageChrome(resource);

  if (resource.dashboard) {
    await renderDashboard();
    return;
  }

  try {
    const data = await apiFetch(resource.endpoint);
    if (loadId !== state.pageLoadId || state.page !== page) return;
    renderTable(Array.isArray(data) ? data : [data], resource);
  } catch (error) {
    if (loadId !== state.pageLoadId || state.page !== page) return;
    renderTable([], resource);
    showAlert(`Erro ao carregar dados: ${error.message}`, 'danger');
  }
}

async function renderDashboard() {
  els.tableHead.innerHTML = '';
  els.tableBody.innerHTML = '';

  const cards = dashboardCards();
  const requestsPanel = ['Admin', 'Diretor'].includes(state.role)
    ? await renderPendingRequestsPanel()
    : '';

  els.dashboardArea.innerHTML = `
    <div class="dashboard-layout ${requestsPanel ? 'has-requests' : 'quick-only'}">
      ${requestsPanel}
      <section class="quick-actions">
        <div class="section-title">
          <h3>Ações rápidas</h3>
        </div>
        <div class="dashboard-grid">
          ${cards.map(card => `
            <button class="btn btn-light dashboard-card" type="button" data-dashboard-action="${card.action}">
              <span class="action-mark">${escapeHtml(card.initial)}</span>
              <span class="action-copy">
                <strong>${escapeHtml(card.title)}</strong>
                <small>${escapeHtml(card.description)}</small>
              </span>
            </button>
          `).join('')}
        </div>
      </section>
    </div>
  `;

  els.dashboardArea.querySelectorAll('[data-dashboard-action]').forEach(button => {
    button.addEventListener('click', () => handleDashboardAction(button.dataset.dashboardAction));
  });

  els.dashboardArea.querySelectorAll('[data-request-details]').forEach(button => {
    button.addEventListener('click', () => openRequestDetails(button.dataset.requestDetails));
  });
}

function dashboardCards() {
  const byRole = {
    Admin: [
      { initial: 'S', title: 'Solicitações pendentes', description: 'Analisar pedidos de acesso', action: 'page:solicitacoes' },
      { initial: 'M', title: 'Matricular aluno', description: 'Aluno em uma disciplina', action: 'create:matriculas' },
      { initial: 'N', title: 'Lançar nota', description: 'Nota para aluno matriculado', action: 'create:notas' },
      { initial: 'U', title: 'Gerenciar usuários', description: 'Contas e perfis do sistema', action: 'page:usuarios' },
    ],
    Diretor: [
      { initial: 'S', title: 'Solicitações pendentes', description: 'Aprovar ou recusar acessos', action: 'page:solicitacoes' },
      { initial: 'M', title: 'Matricular aluno', description: 'Aluno em uma disciplina', action: 'create:matriculas' },
      { initial: 'A', title: 'Gerenciar alunos', description: 'Cadastros e situação acadêmica', action: 'page:alunos' },
      { initial: 'P', title: 'Gerenciar professores', description: 'Equipe docente cadastrada', action: 'page:professores' },
    ],
    Professor: [
      { initial: 'N', title: 'Lançar nota', description: 'Selecionar disciplina e aluno', action: 'create:notas' },
      { initial: 'A', title: 'Meus alunos', description: 'Alunos das suas disciplinas', action: 'page:alunos' },
    ],
    Aluno: [
      { initial: 'D', title: 'Minhas disciplinas', description: 'Disciplinas matriculadas', action: 'page:alunoPortal' },
      { initial: 'B', title: 'Meu boletim', description: 'Notas e situação acadêmica', action: 'page:boletim' },
    ],
  };

  return byRole[state.role] || [];
}

async function renderPendingRequestsPanel() {
  try {
    const requests = await apiFetch('/api/SolicitacaoAcesso?status=Pendente');
    const items = Array.isArray(requests) ? requests : [];
    const visibleItems = items.slice(0, 5);

    return `
      <section class="requests-widget">
        <div class="requests-widget-head">
          <div>
            <h3>Solicitações pendentes</h3>
            <p>${items.length ? `${items.length} aguardando análise` : 'Nenhuma solicitação pendente'}</p>
          </div>
          <button class="btn btn-sm btn-outline-secondary" type="button" data-dashboard-action="page:solicitacoes">Ver todas</button>
        </div>
        <div class="request-list">
          ${visibleItems.length ? visibleItems.map(request => `
            <article class="request-row">
              <div>
                <strong>${escapeHtml(request.nome)}</strong>
                <span>${escapeHtml(request.tipoSolicitado)}</span>
              </div>
              <button class="btn btn-sm btn-outline-primary" type="button" data-request-details="${request.id}">Ver mais</button>
            </article>
          `).join('') : '<p class="empty-state">A lista fica vazia quando todos os pedidos são aceitos ou recusados.</p>'}
        </div>
      </section>
    `;
  } catch (error) {
    return `<section class="requests-widget"><p class="empty-state">Erro ao carregar solicitações: ${escapeHtml(error.message)}</p></section>`;
  }
}

async function handleDashboardAction(action) {
  const [type, page] = action.split(':');
  if (!resources[page]?.roles.includes(state.role)) return;

  state.page = page;
  renderShell();
  await loadCurrentPage();

  if (type === 'create') {
    await openCreateModal();
  }
}

function renderTable(items, resource = currentResource()) {
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
  if (resource.page === 'solicitacoes') {
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
    if (field.type !== 'select-entity' && field.type !== 'select-enrolled') return field;
    const data = await apiFetch(field.endpoint);
    const options = Array.isArray(data) ? data.map(option => ({
      value: option.id,
      label: option.nome || option.email || `Registro ${option.id}`,
      alunoId: option.alunoId,
      alunoNome: option.alunoNome,
      disciplinaId: option.disciplinaId,
    })) : [];
    return { ...field, options };
  }));

  els.modalBody.innerHTML = preparedFields.map(field => {
    const value = item[field.name] ?? '';
    if (field.type === 'select') {
      return `<div class="mb-3"><label class="form-label">${field.label}</label><select class="form-select" name="${field.name}" ${field.required ? 'required' : ''}>${field.options.map(option => {
        const optionValue = typeof option === 'object' ? option.value : option;
        const optionLabel = typeof option === 'object' ? option.label : option;
        return `<option value="${escapeHtml(String(optionValue))}" ${optionValue === value ? 'selected' : ''}>${escapeHtml(String(optionLabel))}</option>`;
      }).join('')}</select></div>`;
    }
    if (field.type === 'select-entity') {
      return `<div class="mb-3">
        <label class="form-label">${field.label}</label>
        <select class="form-select" name="${field.name}" ${field.required ? 'required' : ''}>
          <option value="">Nenhum</option>
          ${field.options.map(o => `<option value="${o.value}" ${String(o.value) === String(value) ? 'selected' : ''}>${escapeHtml(o.label)}</option>`).join('')}
        </select>
      </div>`;
    }
    if (field.type === 'select-enrolled') {
      return `<div class="mb-3">
        <label class="form-label">${field.label}</label>
        <select class="form-select" name="${field.name}" ${field.required ? 'required' : ''}>
          <option value="">Selecione uma disciplina primeiro</option>
          ${field.options.map(o => `<option value="${o.alunoId}" data-disciplina-id="${o.disciplinaId}" ${String(o.alunoId) === String(value) && String(o.disciplinaId) === String(item.disciplinaId) ? 'selected' : ''}>${escapeHtml(o.alunoNome || o.label)}</option>`).join('')}
        </select>
      </div>`;
    }
    return `<div class="mb-3">
      <label class="form-label">${field.label}</label>
      <input class="form-control" name="${field.name}" type="${field.type}" value="${escapeHtml(String(value))}" ${field.required ? 'required' : ''} ${field.min !== undefined ? `min="${field.min}"` : ''} ${field.max !== undefined ? `max="${field.max}"` : ''} ${field.step ? `step="${field.step}"` : ''}>
    </div>`;
  }).join('');

  configurarFiltroAlunosMatriculados();
}

function formPayload() {
  const resource = resources[state.page];
  const data = new FormData(els.entityForm);
  const payload = {};
  (resource.fields || []).forEach(field => {
    const raw = data.get(field.name);
    if (field.type === 'number' || field.type === 'select-entity' || field.type === 'select-enrolled') payload[field.name] = raw === '' ? null : Number(raw);
    else payload[field.name] = String(raw || '').trim();
  });

  if (state.page === 'disciplinas') {
    const professorSelect = els.entityForm.querySelector('[name="professorId"]');
    payload.professorNome = professorSelect?.selectedOptions[0]?.textContent?.trim() || '';
  }

  return payload;
}

function configurarFiltroAlunosMatriculados() {
  const disciplinaSelect = els.modalBody.querySelector('[name="disciplinaId"]');
  const alunoSelect = els.modalBody.querySelector('[name="alunoId"]');
  if (!disciplinaSelect || !alunoSelect || !alunoSelect.querySelector('[data-disciplina-id]')) return;

  const aplicarFiltro = () => {
    const disciplinaId = disciplinaSelect.value;
    let selecionadoVisivel = false;

    Array.from(alunoSelect.options).forEach(option => {
      if (!option.dataset.disciplinaId) return;

      const visivel = option.dataset.disciplinaId === disciplinaId;
      option.hidden = !visivel;
      option.disabled = !visivel;
      if (visivel && option.selected) selecionadoVisivel = true;
    });

    if (!selecionadoVisivel) alunoSelect.value = '';
  };

  disciplinaSelect.addEventListener('change', aplicarFiltro);
  aplicarFiltro();
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
  els.requestDetailsModal.hide();
  els.approvalRequestId.value = id;
  els.approvalPassword.value = '';
  els.approvalPasswordConfirm.value = '';
  els.approvalAlert.innerHTML = '';
  els.approvalModal.show();
}

async function submitApproval(event) {
  event.preventDefault();
  const id = els.approvalRequestId.value;
  const senhaInicial = els.approvalPassword.value;
  const confirmacao = els.approvalPasswordConfirm.value;

  if (!senhaInicial.trim()) {
    showAlert('Informe uma senha inicial para aprovar a solicitação.', 'danger', els.approvalAlert);
    return;
  }

  if (senhaInicial.length < 6) {
    showAlert('A senha inicial precisa ter pelo menos 6 caracteres.', 'danger', els.approvalAlert);
    return;
  }

  if (senhaInicial !== confirmacao) {
    showAlert('A confirmação de senha não confere.', 'danger', els.approvalAlert);
    return;
  }

  try {
    const result = await apiFetch(`/api/SolicitacaoAcesso/${id}/aprovar`, { method: 'POST', body: JSON.stringify({ senhaInicial }) });
    els.approvalModal.hide();
    showAlert(`Solicitação aprovada. Senha temporária: ${result.senhaTemporaria}`, 'success');
    await loadCurrentPage();
  } catch (error) {
    showAlert(`Erro ao aprovar: ${error.message}`, 'danger', els.approvalAlert);
  }
}

async function rejectRequest(id) {
  try {
    els.requestDetailsModal.hide();
    await apiFetch(`/api/SolicitacaoAcesso/${id}/recusar`, { method: 'POST' });
    showAlert('Solicitação recusada.', 'success');
    await loadCurrentPage();
  } catch (error) {
    showAlert(`Erro ao recusar: ${error.message}`, 'danger');
  }
}

async function openRequestDetails(id) {
  try {
    const request = await apiFetch(`/api/SolicitacaoAcesso/${id}`);
    const disabled = request.status !== 'Pendente';

    els.requestDetailsBody.innerHTML = `
      <dl class="request-details">
        <div>
          <dt>Nome</dt>
          <dd>${escapeHtml(request.nome)}</dd>
        </div>
        <div>
          <dt>Email</dt>
          <dd>${escapeHtml(request.email)}</dd>
        </div>
        <div>
          <dt>Tipo</dt>
          <dd>${escapeHtml(request.tipoSolicitado)}</dd>
        </div>
        <div>
          <dt>Status</dt>
          <dd>${escapeHtml(request.status)}</dd>
        </div>
        <div>
          <dt>Mensagem</dt>
          <dd>${escapeHtml(request.mensagem || 'Sem mensagem.')}</dd>
        </div>
      </dl>
    `;

    els.requestApproveBtn.disabled = disabled;
    els.requestRejectBtn.disabled = disabled;
    els.requestApproveBtn.onclick = () => approveRequest(request.id);
    els.requestRejectBtn.onclick = () => rejectRequest(request.id);
    els.requestDetailsModal.show();
  } catch (error) {
    showAlert(`Erro ao carregar solicitação: ${error.message}`, 'danger');
  }
}

function escapeHtml(value) {
  return value.replaceAll('&', '&amp;').replaceAll('<', '&lt;').replaceAll('>', '&gt;').replaceAll('"', '&quot;').replaceAll("'", '&#039;');
}

window.openEditModal = openEditModal;
window.deleteItem = deleteItem;
window.approveRequest = approveRequest;
window.rejectRequest = rejectRequest;
window.openRequestDetails = openRequestDetails;

els.loginForm.addEventListener('submit', login);
els.accessForm.addEventListener('submit', sendAccessRequest);
els.logoutBtn.addEventListener('click', clearAuth);
els.newBtn.addEventListener('click', openCreateModal);
els.entityForm.addEventListener('submit', submitForm);
els.approvalForm.addEventListener('submit', submitApproval);
document.getElementById('showAccessRequestBtn').addEventListener('click', () => showAuth('access'));
document.getElementById('showLoginBtn').addEventListener('click', () => showAuth('login'));
document.querySelectorAll('.preset-login').forEach(btn => btn.addEventListener('click', () => {
  els.emailInput.value = btn.dataset.email;
  els.senhaInput.value = '123456';
}));

renderShell();
if (state.token) loadCurrentPage();
