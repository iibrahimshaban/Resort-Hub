function viewUser(userId) {
    fetch(`/Admin/Users/Details/${userId}`, {
        headers: { 'X-Requested-With': 'XMLHttpRequest' }
    })
        .then(response => response.text())
        .then(html => {
            document.getElementById('userDetails').innerHTML = html;
            document.getElementById('userDetailsModal').classList.add('show');
        });
}

function editUser(userId) {
    const newRole = confirm('Assign Admin role to this user?');
    if (newRole) {
        updateUserRole(userId, 'Admin');
    }
}

function toggleUserStatus(userId) {
    fetch('/Admin/Users/ToggleStatus', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': getAntiForgeryToken()
        },
        body: JSON.stringify(userId)
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) location.reload();
        });
}

function closeModal() {
    document.getElementById('userDetailsModal').classList.remove('show');
}

function getAntiForgeryToken() {
    return document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
}