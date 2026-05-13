const state = {
  token: localStorage.getItem('sprint3_token') || '',
  role: localStorage.getItem('sprint3_role') || '',
  email: localStorage.getItem('sprint3_email') || '',
  page: 'alunos',
  editingId: null,
};

const resources = {
  alunos: {
    title: 'Alunos',
    subtitle: 'Gerencie os alunos cadastrados.',
    endpoint: '/api/Aluno',
    rolesWrite: ['Admin'],
    fields: [
      { name: 'nome', label: 'Nome', type: 'text', required: true },
      { name: 'email', label: 'Email', type: 'email', required: true },
    ],
    columns: [
      { key: 'id', label: 'ID' },
      { key: 'nome', label: 'Nome' },
      { key: 'email', label: 'Email' },
      { key: 'media', label: 'Média' },
      { key: 'situacao', label: 'Situação' },
      { key: 'notas', label: 'Notas', format: value => Array.isArray(value) ? value.join(', ') || '-' : '-' },
    ],
  },
  professores: {
    title: 'Professores',
    subtitle: 'Gerencie os professores cadastrados.',
    endpoint: '/api/Professor',
    rolesWrite: ['Admin'],
    fields: [
      { name: 'nome', label: 'Nome', type: 'text', required: true },
      { name: 'email', label: 'Email', type: 'email', required: true },
    ],
    columns: [
      { key: 'id', label: 'ID' },
      { key: 'nome', label: 'Nome' },
      { key: 'email', label: 'Email' },
      { key: 'disciplinas', label: 'Disciplinas', format: value => Array.isArray(value) ? value.join(', ') || '-' : '-' },
    ],
  },
  disciplinas: {
    title: 'Disciplinas',
    subtitle: 'Gerencie as disciplinas e seus professores.',
    endpoint: '/api/Disciplina',
    rolesWrite: ['Admin', 'Professor'],
    fields: [
      { name: 'nome', label: 'Nome da disciplina', type: 'text', required: true },
      { name: 'professorNome', label: 'Nome do professor', type: 'text', required: true, help: 'Informe o nome do professor já cadastrado.' },
    ],
    columns: [
      { key: 'id', label: 'ID' },
      { key: 'nome', label: 'Disciplina' },
      { key: 'professorId', label: 'Professor ID' },
      { key: 'professorNome', label: 'Professor' },
    ],
  },
  notas: {
    title: 'Notas',
    subtitle: 'Gerencie notas por aluno e disciplina.',
    endpoint: '/api/Nota',
    rolesWrite: ['Admin', 'Professor'],
    fields: [
      { name: 'valor', label: 'Valor', type: 'number', step: '0.1', min: '0', max: '10', required: true },
      { name: 'alunoNome', label: 'Nome do aluno', type: 'text', required: true, help: 'Informe o nome do aluno já cadastrado.' },
      { name: 'disciplinaNome', label: 'Nome da disciplina', type: 'text', required: true, help: 'Informe o nome da disciplina já cadastrada.' },
    ],
    columns: [
      { key: 'id', label: 'ID' },
      { key: 'valor', label: 'Valor' },
      { key: 'alunoId', label: 'Aluno ID' },
      { key: 'alunoNome', label: 'Aluno' },
      { key: 'disciplinaId', label: 'Disciplina ID' },
      { key: 'disciplinaNome', label: 'Disciplina' },
    ],
  },
};

const els = {
  emailInput: document.getElementById('emailInput'),
  senhaInput: document.getElementById('senhaInput'),
  loginBtn: document.getElementById('loginBtn'),
  logoutBtn: document.getElementById('logoutBtn'),
  userBadge: document.getElementById('userBadge'),
  alertArea: document.getElementById('alertArea'),
  pageTitle: document.getElementById('pageTitle'),
  pageSubtitle: document.getElementById('pageSubtitle'),
  tableHead: document.getElementById('tableHead'),
  tableBody: document.getElementById('tableBody'),
  reloadBtn: document.getElementById('reloadBtn'),
  newBtn: document.getElementById('newBtn'),
  modal: new bootstrap.Modal(document.getElementById('entityModal')),
  modalTitle: document.getElementById('modalTitle'),
  modalBody: document.getElementById('modalBody'),
  entityForm: document.getElementById('entityForm'),
};

function normalizeRole(rawRole) {
  if (!rawRole) return '';
  if (Array.isArray(rawRole)) return rawRole[0] || '';
  return rawRole;
}

function decodeJwt(token) {
  try {
    const payload = token.split('.')[1];
    const json = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
    return JSON.parse(decodeURIComponent(escape(json)));
  } catch {
    return {};
  }
}

function saveAuth(token, email) {
  const payload = decodeJwt(token);
  const role = normalizeRole(
    payload.role ||
    payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
  );

  state.token = token;
  state.role = role;
  state.email = email;

  localStorage.setItem('sprint3_token', token);
  localStorage.setItem('sprint3_role', role);
  localStorage.setItem('sprint3_email', email);
  updateAuthUi();
}

function clearAuth() {
  state.token = '';
  state.role = '';
  state.email = '';
  localStorage.removeItem('sprint3_token');
  localStorage.removeItem('sprint3_role');
  localStorage.removeItem('sprint3_email');
  updateAuthUi();
}

function updateAuthUi() {
  if (state.token) {
    els.userBadge.textContent = `${state.role || 'Usuário'} - ${state.email}`;
    els.userBadge.className = 'badge text-bg-success badge-role';
    els.logoutBtn.classList.remove('d-none');
  } else {
    els.userBadge.textContent = 'Não autenticado';
    els.userBadge.className = 'badge text-bg-secondary';
    els.logoutBtn.classList.add('d-none');
  }
  updateNewButton();
}

function showAlert(message, type = 'info') {
  els.alertArea.innerHTML = `
    <div class="alert alert-${type} alert-dismissible fade show" role="alert">
      ${message}
      <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Fechar"></button>
    </div>
  `;
}

async function apiFetch(url, options = {}) {
  const headers = {
    'Content-Type': 'application/json',
    ...(options.headers || {}),
  };

  if (state.token) {
    headers.Authorization = `Bearer ${state.token}`;
  }

  const response = await fetch(url, { ...options, headers });

  if (response.status === 204) return null;

  const contentType = response.headers.get('content-type') || '';
  const data = contentType.includes('application/json') ? await response.json() : await response.text();

  if (!response.ok) {
    const message = typeof data === 'string' ? data : (data.mensagem || data.title || 'Erro na requisição.');
    throw new Error(`${response.status} - ${message}`);
  }

  return data;
}

async function login() {
  const email = els.emailInput.value.trim();
  const senha = els.senhaInput.value.trim();

  try {
    const data = await apiFetch('/api/Auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, senha }),
      headers: {},
    });

    saveAuth(data.token, email);
    showAlert('Login realizado com sucesso.', 'success');
    await loadCurrentPage();
  } catch (error) {
    showAlert(`Falha no login: ${error.message}`, 'danger');
  }
}

function canWrite(resource) {
  return state.role && resource.rolesWrite.includes(state.role);
}

function updateNewButton() {
  const resource = resources[state.page];
  if (!resource) return;
  els.newBtn.disabled = !canWrite(resource);
  els.newBtn.title = canWrite(resource) ? '' : 'Seu perfil não tem permissão para cadastrar neste módulo.';
}

function setPage(page) {
  state.page = page;
  document.querySelectorAll('[data-page]').forEach(button => {
    button.classList.toggle('active', button.dataset.page === page);
  });
  loadCurrentPage();
}

function renderTable(items) {
  const resource = resources[state.page];
  const columns = [...resource.columns, { key: 'actions', label: 'Ações' }];

  els.tableHead.innerHTML = `
    <tr>${columns.map(column => `<th>${column.label}</th>`).join('')}</tr>
  `;

  if (!items.length) {
    els.tableBody.innerHTML = `<tr><td colspan="${columns.length}" class="text-center text-muted py-4">Nenhum registro encontrado.</td></tr>`;
    return;
  }

  els.tableBody.innerHTML = items.map(item => {
    const cells = resource.columns.map(column => {
      const rawValue = item[column.key];
      const value = column.format ? column.format(rawValue, item) : (rawValue ?? '-');
      return `<td>${escapeHtml(String(value))}</td>`;
    }).join('');

    const disabled = canWrite(resource) ? '' : 'disabled';
    return `
      <tr>
        ${cells}
        <td class="action-cell">
          <button class="btn btn-sm btn-outline-primary me-1" ${disabled} onclick="openEditModal(${escapeHtml(String(item.id))})">Editar</button>
          <button class="btn btn-sm btn-outline-danger" ${disabled} onclick="deleteItem(${escapeHtml(String(item.id))})">Excluir</button>
        </td>
      </tr>
    `;
  }).join('');
}

function escapeHtml(value) {
  return value
    .replaceAll('&', '&amp;')
    .replaceAll('<', '&lt;')
    .replaceAll('>', '&gt;')
    .replaceAll('"', '&quot;')
    .replaceAll("'", '&#039;');
}

async function loadCurrentPage() {
  const resource = resources[state.page];
  els.pageTitle.textContent = resource.title;
  els.pageSubtitle.textContent = resource.subtitle;
  updateNewButton();

  try {
    const data = await apiFetch(resource.endpoint);
    renderTable(Array.isArray(data) ? data : [data]);
  } catch (error) {
    renderTable([]);
    showAlert(`Erro ao carregar ${resource.title.toLowerCase()}: ${error.message}`, 'danger');
  }
}

function openCreateModal() {
  const resource = resources[state.page];
  if (!canWrite(resource)) {
    showAlert('Seu perfil não tem permissão para cadastrar neste módulo.', 'warning');
    return;
  }

  state.editingId = null;
  els.modalTitle.textContent = `Novo registro - ${resource.title}`;
  renderForm(resource.fields, {});
  els.modal.show();
}

async function openEditModal(id) {
  const resource = resources[state.page];
  if (!canWrite(resource)) {
    showAlert('Seu perfil não tem permissão para editar neste módulo.', 'warning');
    return;
  }

  try {
    const item = await apiFetch(`${resource.endpoint}/${id}`);
    state.editingId = id;
    els.modalTitle.textContent = `Editar registro - ${resource.title}`;
    renderForm(resource.fields, item);
    els.modal.show();
  } catch (error) {
    showAlert(`Erro ao buscar registro: ${error.message}`, 'danger');
  }
}

function renderForm(fields, item) {
  els.modalBody.innerHTML = fields.map(field => {
    const value = item[field.name] ?? '';
    const step = field.step ? `step="${field.step}"` : '';
    const min = field.min ? `min="${field.min}"` : '';
    const max = field.max ? `max="${field.max}"` : '';
    const required = field.required ? 'required' : '';
    const help = field.help ? `<div class="form-text form-help">${field.help}</div>` : '';

    return `
      <div class="mb-3">
        <label class="form-label" for="field-${field.name}">${field.label}</label>
        <input class="form-control" id="field-${field.name}" name="${field.name}" type="${field.type}" value="${escapeHtml(String(value))}" ${step} ${min} ${max} ${required}>
        ${help}
      </div>
    `;
  }).join('');
}

function getFormData() {
  const resource = resources[state.page];
  const formData = new FormData(els.entityForm);
  const payload = {};

  resource.fields.forEach(field => {
    const value = formData.get(field.name);
    payload[field.name] = field.type === 'number' ? Number(value) : String(value).trim();
  });

  return payload;
}

async function submitForm(event) {
  event.preventDefault();
  const resource = resources[state.page];
  const payload = getFormData();
  const method = state.editingId ? 'PUT' : 'POST';
  const url = state.editingId ? `${resource.endpoint}/${state.editingId}` : resource.endpoint;

  try {
    await apiFetch(url, {
      method,
      body: JSON.stringify(payload),
    });

    els.modal.hide();
    showAlert('Registro salvo com sucesso.', 'success');
    await loadCurrentPage();
  } catch (error) {
    showAlert(`Erro ao salvar: ${error.message}`, 'danger');
  }
}

async function deleteItem(id) {
  const resource = resources[state.page];
  if (!canWrite(resource)) {
    showAlert('Seu perfil não tem permissão para excluir neste módulo.', 'warning');
    return;
  }

  if (!confirm('Deseja realmente excluir este registro?')) return;

  try {
    await apiFetch(`${resource.endpoint}/${id}`, { method: 'DELETE' });
    showAlert('Registro removido com sucesso.', 'success');
    await loadCurrentPage();
  } catch (error) {
    showAlert(`Erro ao excluir: ${error.message}`, 'danger');
  }
}

window.openEditModal = openEditModal;
window.deleteItem = deleteItem;

els.loginBtn.addEventListener('click', login);
els.logoutBtn.addEventListener('click', () => {
  clearAuth();
  showAlert('Logout realizado.', 'info');
  loadCurrentPage();
});
els.reloadBtn.addEventListener('click', loadCurrentPage);
els.newBtn.addEventListener('click', openCreateModal);
els.entityForm.addEventListener('submit', submitForm);

document.querySelectorAll('[data-page]').forEach(button => {
  button.addEventListener('click', () => setPage(button.dataset.page));
});

document.querySelectorAll('.preset-login').forEach(button => {
  button.addEventListener('click', () => {
    els.emailInput.value = button.dataset.email;
    els.senhaInput.value = '123456';
  });
});

updateAuthUi();
loadCurrentPage();
