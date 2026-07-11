function confirmDelete(id, name) {
    Swal.fire({
        title: 'متأكد؟', text: `هيتم حذف "${name}"`, icon: 'warning',
        showCancelButton: true, confirmButtonText: 'أيوه، احذف', cancelButtonText: 'إلغاء',
        confirmButtonColor: '#a03a2c', cancelButtonColor: '#6c757d'
    }).then((result) => { if (result.isConfirmed) performDelete(id); });
}
function performDelete(id) {
    fetch(`/Book/DeleteAjax/${id}`, {
        method: 'POST',
        headers: { 'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value ?? '' }
    })
        .then(res => res.json())
        .then(data => {
            if (data.success) { document.getElementById(`cat-card-${id}`).remove(); Swal.fire('تم الحذف', '', 'success'); }
            else Swal.fire('خطأ', data.message, 'error');
        });
}

function goToBookDetails(e, id) {
    if (e.target.closest('.cat-card__actions')) return;
    window.location.href = `/Book/Details/${id}`;
}