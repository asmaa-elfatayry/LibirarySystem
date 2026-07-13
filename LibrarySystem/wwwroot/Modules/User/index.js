function onRoleChange(userId, selectEl) {
    const row = selectEl.closest('.loans-table__row');
    const originalRole = row.dataset.originalRole;
    const saveBtn = document.getElementById(`save-role-${userId}`);

    saveBtn.style.display = selectEl.value !== originalRole ? 'inline-flex' : 'none';
}

function saveRole(userId) {
    const row = document.getElementById(`user-row-${userId}`);
    const selectEl = row.querySelector('.copy-row__status');
    const saveBtn = document.getElementById(`save-role-${userId}`);
    const newRole = selectEl.value;

    selectEl.disabled = true;
    saveBtn.disabled = true;

    fetch(`/User/ChangeRoleAjax?userId=${userId}&newRole=${newRole}`, {
        method: 'POST',
        headers: { 'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value ?? '' }
    })
        .then(res => res.json())
        .then(data => {
            selectEl.disabled = false;
            saveBtn.disabled = false;

            if (data.success) {
                selectEl.className = `copy-row__status status-${newRole.toLowerCase()}`;
                row.dataset.originalRole = newRole;
                saveBtn.style.display = 'none';
            } else {
                Swal.fire('خطأ', data.message, 'error');
                selectEl.value = row.dataset.originalRole; // رجّع الاختيار القديم
            }
        });
}