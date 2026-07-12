function returnLoan(id) {
    Swal.fire({
        title: 'تأكيد الإرجاع؟',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'أيوه',
        cancelButtonText: 'إلغاء'
    }).then((result) => {
        if (!result.isConfirmed) return;

        fetch(`/Loan/ReturnAjax?id=${id}`, {
            method: 'POST',
            headers: { 'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value ?? '' }
        })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    window.location.reload();
                } else {
                    Swal.fire('خطأ', data.message ?? 'حصل خطأ', 'error');
                }
            });
    });
}