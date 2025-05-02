// document.addEventListener('DOMContentLoaded', function() {
//     const serviceInput = document.getElementById('serviceInput');
//     const servicePopup = document.getElementById('servicePopup');
//     const options = document.querySelectorAll('.option');
//     const calendarInput = document.getElementById('calendar-input');
//
//     options.forEach(option => {
//         option.addEventListener('click', function() {
//             options.forEach(opt => opt.classList.remove('selected'));
//
//             this.classList.add('selected');
//
//             const selectedValue = this.getAttribute('data-value');
//             serviceInput.value = selectedValue;
//
//             servicePopup.classList.remove('show');
//         });
//     });
//
//     serviceInput.addEventListener('click', (e) => {
//         e.stopPropagation();
//         servicePopup.classList.add('show');
//     });
//
//     servicePopup.addEventListener('click', (e) => {
//         e.stopPropagation();
//     });
//    
//     calendarInput.addEventListener('click', (e) => {
//         servicePopup.classList.remove('show');
//     })
//    
//
//     document.addEventListener('click', (e) => {
//         if (!servicePopup.contains(e.target) && e.target !== serviceInput) {
//             servicePopup.classList.remove('show');
//         }
//        
//     });
// });