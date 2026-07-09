function confirmDelete(id, name) {
    Swal.fire({
        title: 'متأكد؟',
        text: `هيتم حذف الناشر "${name}" نهائيًا`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'أيوه، احذف',
        cancelButtonText: 'إلغاء',
        confirmButtonColor: '#a03a2c',
        cancelButtonColor: '#6c757d'
    }).then((result) => {
        if (result.isConfirmed) performDelete(id);
    });
}

function performDelete(id) {
    fetch(`/Publisher/DeleteAjax/${id}`, {
        method: 'POST',
        headers: { 'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value ?? '' }
    })
        .then(res => { if (!res.ok) throw new Error(); return res.json(); })
        .then(data => {
            if (data.success) {
                document.getElementById(`cat-card-${id}`).remove();
                Swal.fire('تم الحذف', '', 'success');
            } else {
                Swal.fire('حصل خطأ', data.message ?? 'حاول تاني', 'error');
            }
        })
        .catch(() => Swal.fire('حصل خطأ', 'مقدرش يوصل للسيرفر', 'error'));
}