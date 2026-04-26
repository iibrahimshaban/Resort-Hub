let currentUserId = null;
let currentUserRoles = [];

// ─── Toast ────────────────────────────────────────────────────────────────────

function showToast(message, type = 'success') {
    let toast = document.getElementById('toast');
    if (!toast) {
        toast = document.createElement('div');
        toast.id = 'toast';
        toast.className = 'toast hidden';
        toast.innerHTML = `
            <div class="toast-content">
                <i class="fas" id="toastIcon"></i>
                <span id="toastMessage"></span>
            </div>`;
        document.body.appendChild(toast);
    }

    const toastIcon = document.getElementById('toastIcon');
    const toastMessage = document.getElementById('toastMessage');

    toastIcon.className = type === 'success' ? 'fas fa-check-circle' : 'fas fa-exclamation-circle';
    toastMessage.textContent = message;
    toast.classList.add('show');

    setTimeout(() => toast.classList.remove('show'), 3000);
}

// ─── Anti-forgery ─────────────────────────────────────────────────────────────

function getAntiForgeryToken() {
    const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
    if (tokenInput) return tokenInput.value;

    const metaToken = document.querySelector('meta[name="csrf-token"]');
    if (metaToken) return metaToken.getAttribute('content');

    console.warn('Anti-forgery token not found');
    return '';
}

// ─── Modal helpers ────────────────────────────────────────────────────────────

function openModal(modalId) {
    const modal = document.getElementById(modalId);
    if (modal) {
        modal.classList.add('show');
        document.body.style.overflow = 'hidden';
    }
}

function closeModal(modalId) {
    const modal = document.getElementById(modalId);
    if (modal) {
        modal.classList.remove('show');
        document.body.style.overflow = 'auto';
    }
}

// Close any modal when clicking the dark backdrop
window.addEventListener('click', function (event) {
    if (event.target.classList.contains('modal')) {
        event.target.classList.remove('show');
        document.body.style.overflow = 'auto';
    }
});

// ─── View user details ────────────────────────────────────────────────────────

async function viewUser(userId) {
    try {
        const response = await fetch(`/Admin/GetUserById/${userId}`);
        if (!response.ok) throw new Error('User not found');

        const user = await response.json();
        const modalBody = document.getElementById('userDetails');

        modalBody.innerHTML = `
            <div class="user-profile-details">
                <div class="profile-header">
                    <div class="profile-avatar large">${user.avatarInitial}</div>
                    <div class="profile-info">
                        <h4>${user.fullName}</h4>
                        <p><i class="fas fa-envelope"></i> ${user.email}</p>
                        <p><i class="fas fa-phone"></i> ${user.phoneNumber || 'Not provided'}</p>
                    </div>
                </div>
                <div class="profile-details-grid">
                    <div class="detail-item">
                        <label>Joined Date:</label>
                        <span>${new Date(user.createdAt).toLocaleDateString()}</span>
                    </div>
                    <div class="detail-item">
                        <label>Total Bookings:</label>
                        <span>${user.bookingsCount}</span>
                    </div>
                    <div class="detail-item">
                        <label>Roles:</label>
                        <span>${user.roles.join(', ') || 'No roles assigned'}</span>
                    </div>
                    <div class="detail-item">
                        <label>Email Confirmed:</label>
                        <span>${user.emailConfirmed ? 'Yes' : 'No'}</span>
                    </div>
                    <div class="detail-item">
                        <label>Status:</label>
                        <span class="status-badge ${user.isActive ? 'status-active' : 'status-inactive'}">
                            ${user.isActive ? 'Active' : 'Inactive'}
                        </span>
                    </div>
                </div>
            </div>`;

        openModal('userDetailsModal');
    } catch (error) {
        console.error('Error fetching user details:', error);
        showToast('Error loading user details', 'error');
    }
}

// ─── Edit user role (opens the Edit Role modal) ───────────────────────────────

async function editUser(userId) {
    try {
        const response = await fetch(`/Admin/GetUserById/${userId}`);
        if (!response.ok) throw new Error('User not found');

        const user = await response.json();

        currentUserId = userId;
        currentUserRoles = user.roles;

        document.getElementById('userFullName').value = user.fullName;

        const roleSelect = document.getElementById('userRoleSelect');
        const knownRoles = ['Admin', 'Customer'];
        const activeRole = user.roles.find(r => knownRoles.includes(r)) ?? 'Customer';
        roleSelect.value = activeRole;

        openModal('editRoleModal');
    } catch (error) {
        console.error('Error loading user for edit:', error);
        showToast('Error loading user data', 'error');
    }
}

window.editUserRole = editUser;


async function updateUserRole() {
    const newRole = document.getElementById('userRoleSelect').value;

    if (!currentUserId) {
        showToast('No user selected', 'error');
        return;
    }

    try {
        const response = await fetch('/Admin/UpdateUserRole', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getAntiForgeryToken()
            },
            body: JSON.stringify({ userId: currentUserId, role: newRole })
        });

        const result = await response.json();

        if (result.success) {
            closeModal('editRoleModal');
            showToast(`Role updated to '${newRole}' successfully`, 'success');
            setTimeout(() => location.reload(), 1500);
        } else {
            showToast(result.message || 'Failed to update role', 'error');
        }
    } catch (error) {
        console.error('Error updating role:', error);
        showToast('Error updating role', 'error');
    }
}


async function toggleUserStatus(userId, currentIsActive) {
    const action = currentIsActive ? 'deactivate' : 'activate';

    if (!confirm(`Are you sure you want to ${action} this user?`)) return;

    try {
        const response = await fetch('/Admin/ToggleUserStatus', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getAntiForgeryToken()
            },
            body: JSON.stringify({ userId: userId })
        });

        const result = await response.json();

        if (result.success) {
            showToast(`User ${result.isActive ? 'activated' : 'deactivated'} successfully`, 'success');
            setTimeout(() => location.reload(), 1000);
        } else {
            showToast(result.message || 'Failed to update status', 'error');
        }
    } catch (error) {
        console.error('Error toggling user status:', error);
        showToast('Error updating user status', 'error');
    }
}

// ─── Pagination ───────────────────────────────────────────────────────────────

document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.paginate').forEach(link => {
        link.addEventListener('click', e => {
            e.preventDefault();
            const page = link.dataset.page;
            if (!page) return;
            const urlParams = new URLSearchParams(window.location.search);
            urlParams.set('page', page);
            window.location.href = `/Admin/Users?${urlParams.toString()}`;
        });
    });

    document.querySelectorAll('.sort-link').forEach(link => {
        link.addEventListener('click', e => {
            e.preventDefault();
            const sortBy = link.dataset.sort;
            const urlParams = new URLSearchParams(window.location.search);
            const currentSort = urlParams.get('sortBy');
            const descending = urlParams.get('descending') === 'true';

            if (currentSort === sortBy) {
                urlParams.set('descending', (!descending).toString());
            } else {
                urlParams.set('sortBy', sortBy);
                urlParams.set('descending', 'false');
            }
            urlParams.set('page', '1');
            window.location.href = `/Admin/Users?${urlParams.toString()}`;
        });
    });
});