function deleteBook(id, title) {
    Swal.fire({
        title: 'متأكد؟',
        text: `هيتم حذف "${title}" وكل نسخه نهائيًا`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'أيوه، احذف',
        cancelButtonText: 'إلغاء',
        confirmButtonColor: '#dc3545',
        cancelButtonColor: '#6c757d'
    }).then((result) => {
        if (!result.isConfirmed) return;

        fetch(`/Book/DeleteAjax/${id}`, {
            method: 'POST',
            headers: { 'RequestVerificationToken': getToken() }
        })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    window.location.href = '/Book/Index';
                } else {
                    Swal.fire('خطأ', data.message ?? 'حصل خطأ', 'error');
                }
            });
    });
}

function deleteCopy(id) {
    Swal.fire({
        title: 'حذف النسخة؟',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'أيوه',
        cancelButtonText: 'إلغاء',
        confirmButtonColor: '#dc3545'
    }).then((result) => {
        if (!result.isConfirmed) return;

        fetch(`/Book/DeleteCopyAjax/${id}`, {
            method: 'POST',
            headers: { 'RequestVerificationToken': getToken() }
        })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    document.getElementById(`copy-row-${id}`).remove();
                } else {
                    Swal.fire('خطأ', data.message ?? 'مقدرش يحذف النسخة', 'error');
                }
            });
    });
}

function onStatusChange(copyId, selectEl) {
    const row = selectEl.closest('.copy-row');
    const originalStatus = row.dataset.originalStatus;
    const saveBtn = document.getElementById(`save-btn-${copyId}`);

    if (selectEl.value !== originalStatus) {
        saveBtn.style.display = 'inline-flex';
        row.classList.add('copy-row--pending');
    } else {
        saveBtn.style.display = 'none';
        row.classList.remove('copy-row--pending');
    }
}

function saveCopyStatus(copyId) {
    const row = document.getElementById(`copy-row-${copyId}`);
    const selectEl = row.querySelector('.copy-row__status');
    const saveBtn = document.getElementById(`save-btn-${copyId}`);
    const newStatus = selectEl.value;

    selectEl.disabled = true;
    saveBtn.disabled = true;
    saveBtn.textContent = '...جاري الحفظ';

    fetch(`/Book/UpdateCopyStatusAjax?id=${copyId}&status=${newStatus}`, {
        method: 'POST',
        headers: { 'RequestVerificationToken': getToken() }
    })
        .then(res => res.json())
        .then(data => {
            selectEl.disabled = false;
            saveBtn.disabled = false;
            saveBtn.textContent = 'حفظ';

            if (data.success) {
                selectEl.className = `copy-row__status status-${newStatus.toLowerCase()}`;
                row.dataset.originalStatus = newStatus;
                saveBtn.style.display = 'none';
                row.classList.remove('copy-row--pending');
                row.classList.add('copy-row--saved');
                setTimeout(() => row.classList.remove('copy-row--saved'), 900);
            } else {
                Swal.fire('خطأ', data.message ?? 'مقدرش يعدّل الحالة', 'error');
            }
        })
        .catch(() => {
            selectEl.disabled = false;
            saveBtn.disabled = false;
            saveBtn.textContent = 'حفظ';
            Swal.fire('خطأ', 'مقدرش يوصل للسيرفر', 'error');
        });
}

document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('addCopyForm');
    const input = form.querySelector('[name="copyNumber"]');
    const submitBtn = form.querySelector('button[type="submit"]');

    form.addEventListener('submit', function (e) {
        e.preventDefault();

        const bookId = form.querySelector('[name="bookId"]').value;
        const copyNumber = input.value.trim();

        submitBtn.disabled = true;
        submitBtn.textContent = 'جاري الإضافة...';

        fetch('/Book/AddCopy', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': getToken()
            },
            body: `bookId=${bookId}&copyNumber=${encodeURIComponent(copyNumber)}`
        })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    window.location.reload();
                } else {
                    submitBtn.disabled = false;
                    submitBtn.textContent = '+ إضافة نسخة';
                    Swal.fire('مقدرش أضيف النسخة', data.message ?? 'حصل خطأ', 'error');
                }
            });
    });
});

function getToken() {
    return document.querySelector('input[name="__RequestVerificationToken"]')?.value ?? '';
}