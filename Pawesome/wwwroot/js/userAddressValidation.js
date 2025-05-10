/**
 * Handles city and postal code validation for user address forms.
 * Ensures the city is selected from suggestions and matches the postal code.
 * Integrates with jQuery Validator for custom validation rules.
 */
document.addEventListener('DOMContentLoaded', function() {
    /**
     * City input element.
     * @type {HTMLInputElement}
     */
    const cityInput = document.getElementById('location-search');
    /**
     * Postal code input element.
     * @type {HTMLInputElement}
     */
    const postalCodeInput = document.getElementById('postal-code');
    /**
     * Search results container element.
     * @type {HTMLElement}
     */
    const searchResults = document.getElementById('search-results');

    /**
     * Stores the selected city.
     * @type {string|null}
     */
    let selectedCity = null;
    /**
     * Stores the selected postal code.
     * @type {string|null}
     */
    let selectedPostalCode = null;
    /**
     * Map of valid city/postal code pairs.
     * @type {Map<string, string>}
     */
    let validCityPostalPairs = new Map();

    /**
     * Indicates if a city was selected from the suggestion list.
     * @type {boolean}
     */
    let citySelectedFromList = false;

    cityInput.addEventListener('input', function() {
        if (citySelectedFromList && cityInput.value !== selectedCity) {
            citySelectedFromList = false;
            postalCodeInput.value = '';
        }
    });

    if (searchResults) {
        searchResults.addEventListener('click', function(event) {
            const resultItem = event.target.closest('.search-result-item');
            if (resultItem) {
                citySelectedFromList = true;
                setTimeout(() => {
                    selectedCity = cityInput.value;
                    selectedPostalCode = postalCodeInput.value;
                    validCityPostalPairs.set(selectedCity, selectedPostalCode);
                }, 10);
            }
        });
    }

    if (window.$ && $.validator) {
        /**
         * Custom validation rule for city/postal code pair.
         * @param {string} value
         * @param {HTMLElement} element
         * @returns {boolean}
         */
        $.validator.addMethod('validCityPostalPair', function(value, element) {
            if (!value.trim() && !$(element).attr('data-val-required')) {
                return true;
            }
            if (citySelectedFromList && value === selectedCity && postalCodeInput.value === selectedPostalCode) {
                return true;
            }
            return validCityPostalPairs.get(value) === postalCodeInput.value && postalCodeInput.value !== '';
        }, 'Please select a city from the suggestion list.');

        $(cityInput).rules('add', {
            validCityPostalPair: true
        });
    }

    const form = cityInput.closest('form');
    if (form) {
        form.addEventListener('submit', function(event) {
            if (cityInput.value.trim() && !citySelectedFromList &&
                (validCityPostalPairs.get(cityInput.value) !== postalCodeInput.value || postalCodeInput.value === '')) {
                event.preventDefault();
                const errorElement = document.querySelector('[data-valmsg-for="City"]');
                if (errorElement) {
                    errorElement.textContent = 'Please select a city from the suggestion list.';
                    errorElement.classList.add('field-validation-error');
                    cityInput.classList.add('input-validation-error');
                }
                cityInput.scrollIntoView({ behavior: 'smooth', block: 'center' });
            }
        });
    }

    if (cityInput.value && postalCodeInput.value) {
        selectedCity = cityInput.value;
        selectedPostalCode = postalCodeInput.value;
        validCityPostalPairs.set(selectedCity, selectedPostalCode);
        citySelectedFromList = true;
    }
});
