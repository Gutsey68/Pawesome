document.addEventListener('DOMContentLoaded', function() {
    const serviceInput = document.getElementById('serviceInput');
    const servicePopup = document.getElementById('servicePopup');
    const options = document.querySelectorAll('.option');

    serviceInput.addEventListener('click', function() {
        servicePopup.classList.add('show');
    });

    options.forEach(option => {
        option.addEventListener('click', function() {
            options.forEach(opt => opt.classList.remove('selected'));

            this.classList.add('selected');

            const selectedValue = this.getAttribute('data-value');
            serviceInput.value = selectedValue;

            servicePopup.classList.remove('show');
        });
    });

    document.addEventListener('click', function(event) {
        if (!serviceInput.contains(event.target) && !servicePopup.contains(event.target)) {
            servicePopup.classList.remove('show');
        }
    });
});