function viewBooking(bookingId) {
    // Fetch booking details
    fetch(`/Admin/Bookings/Details/${bookingId}`)
        .then(response => response.text())
        .then(html => {
            document.getElementById('bookingDetails').innerHTML = html;
            document.getElementById('viewBookingModal').classList.add('show');
        });
}

function closeModal() {
    document.getElementById('viewBookingModal').classList.remove('show');
}

function toggleStatusDropdown(bookingId) {
    const dropdown = document.getElementById(`statusDropdown-${bookingId}`);
    dropdown.classList.toggle('show');
}

function updateStatus(bookingId, statusValue) {
    const data = {
        bookingId: bookingId,
        newStatus: statusValue
    };

    fetch('/Admin/Bookings/UpdateStatus', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': getAntiForgeryToken()
        },
        body: JSON.stringify(data)
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                location.reload();
            } else {
                alert('Failed to update status');
            }
        });
}

function getAntiForgeryToken() {
    const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
    return tokenInput ? tokenInput.value : '';
}

// Close dropdown when clicking outside
document.addEventListener('click', function (event) {
    if (!event.target.closest('.status-dropdown')) {
        document.querySelectorAll('.dropdown-menu').forEach(menu => {
            menu.classList.remove('show');
        });
    }
});