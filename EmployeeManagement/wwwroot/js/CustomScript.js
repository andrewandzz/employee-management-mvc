function toggleDeleteConfirmation(id) {
    var deleteSpan = document.querySelector('#delete_' + id);
    var confirmSpan = document.querySelector('#confirmdelete_' + id);

    if (deleteSpan && confirmSpan) {
        if (deleteSpan.style.display === 'none') {
            deleteSpan.style.display = 'inline-block';
            confirmSpan.style.display = 'none';
        } else {
            deleteSpan.style.display = 'none';
            confirmSpan.style.display = 'inline-block';
        }
    }
}