/**
 * Handles city and postal code search and selection for address forms.
 * Integrates with the French government address API and manages UI updates.
 */
document.addEventListener('DOMContentLoaded', async function() {
    /**
     * City input element.
     * @type {HTMLInputElement}
     */
    const searchInput = document.getElementById('location-search');
    /**
     * Postal code hidden input element.
     * @type {HTMLInputElement}
     */
    const postalCodeInput = document.getElementById('postal-code');
    /**
     * Postal code visible input element.
     * @type {HTMLInputElement}
     */
    const postalCodeVisibleInput = document.getElementById('postal-code-visible');
    /**
     * Search results container element.
     * @type {HTMLElement}
     */
    const searchResults = document.getElementById('search-results');
    let debounceTimer;
    let lastSelectedCity = null;
    let lastSelectedPostalCode = null;

    if (postalCodeVisibleInput && postalCodeInput) {
        postalCodeVisibleInput.value = postalCodeInput.value;

        postalCodeVisibleInput.addEventListener('input', function() {
            postalCodeInput.value = this.value;
            if (this.value.length === 5) {
                searchByPostalCode(this.value);
            }
        });
    }

    /**
     * Searches for cities by name using the French government address API.
     * @param {string} query
     * @returns {Promise<void>}
     */
    async function searchLocation(query) {
        if (query.length < 2) {
            searchResults.style.display = 'none';
            return;
        }
        try {
            const url = `https://api-adresse.data.gouv.fr/search/?q=${encodeURIComponent(query)}&type=municipality&limit=10`;
            const response = await fetch(url);
            const data = await response.json();
            if (data && data.features && data.features.length > 0) {
                displayResults(data.features);
            } else {
                searchResults.style.display = 'none';
            }
        } catch (error) {
            console.error('Search error:', error);
            searchResults.style.display = 'none';
        }
    }

    /**
     * Searches for cities by postal code using the French government address API.
     * @param {string} postalCode
     * @returns {Promise<void>}
     */
    async function searchByPostalCode(postalCode) {
        if (postalCode.length !== 5) {
            return;
        }
        try {
            const url = `https://api-adresse.data.gouv.fr/search/?q=${encodeURIComponent(postalCode)}&type=municipality&limit=5`;
            const response = await fetch(url);
            const data = await response.json();
            if (data && data.features && data.features.length > 0) {
                if (data.features.length === 1) {
                    selectCity(data.features[0]);
                } else {
                    displayResults(data.features);
                }
            }
        } catch (error) {
            console.error('Postal code search error:', error);
        }
    }

    /**
     * Displays the list of city search results.
     * @param {Array} features
     */
    function displayResults(features) {
        searchResults.innerHTML = '';
        if (features.length === 0) {
            searchResults.style.display = 'none';
            return;
        }
        const citiesMap = new Map();
        features.forEach(feature => {
            const city = feature.properties;
            if (!city.postcode || !city.city) return;
            const cityName = city.city;
            if (!citiesMap.has(cityName)) {
                citiesMap.set(cityName, {
                    nom: cityName,
                    postalCodes: new Set([city.postcode])
                });
            } else {
                citiesMap.get(cityName).postalCodes.add(city.postcode);
            }
        });
        const cities = Array.from(citiesMap.values());
        cities.forEach(city => {
            city.postalCodes.forEach(postalCode => {
                const div = document.createElement('div');
                div.className = 'search-result-item';
                div.innerHTML = `
                    <div class="result-main">
                        <div class="city-name">${city.nom} (${postalCode})</div>
                    </div>
                `;
                div.addEventListener('click', () => {
                    selectCity({
                        properties: {
                            city: city.nom,
                            postcode: postalCode
                        }
                    });
                });
                searchResults.appendChild(div);
            });
        });
        if (searchResults.children.length > 0) {
            searchResults.style.display = 'block';
        } else {
            searchResults.style.display = 'none';
        }
    }

    /**
     * Handles the selection of a city from the search results.
     * Updates input fields and triggers validation.
     * @param {Object} feature
     */
    function selectCity(feature) {
        const cityName = feature.properties.city;
        const postalCode = feature.properties.postcode;
        searchInput.value = cityName;
        postalCodeInput.value = postalCode;
        if (postalCodeVisibleInput) {
            postalCodeVisibleInput.value = postalCode;
        }
        lastSelectedCity = cityName;
        lastSelectedPostalCode = postalCode;
        searchInput.dataset.selectedFromList = 'true';
        searchInput.dataset.lastSelectedValue = cityName;
        searchResults.style.display = 'none';
        const changeEvent = new Event('change', { bubbles: true });
        searchInput.dispatchEvent(changeEvent);
        postalCodeInput.dispatchEvent(changeEvent);
        if (postalCodeVisibleInput) {
            postalCodeVisibleInput.dispatchEvent(changeEvent);
        }
        if (window.$ && $.validator) {
            $(searchInput).valid();
            if (postalCodeVisibleInput) {
                $(postalCodeVisibleInput).valid();
            }
        }
    }

    if (searchInput) {
        searchInput.addEventListener('input', function() {
            clearTimeout(debounceTimer);
            const query = this.value.trim();
            if (this.dataset.selectedFromList === 'true' && query !== lastSelectedCity) {
                this.dataset.selectedFromList = 'false';
                postalCodeInput.value = '';
                if (postalCodeVisibleInput) {
                    postalCodeVisibleInput.value = '';
                }
            }
            debounceTimer = setTimeout(() => {
                searchLocation(query);
            }, 300);
        });
    }

    document.addEventListener('click', function(event) {
        if (searchInput && searchResults &&
            !searchInput.contains(event.target) &&
            !searchResults.contains(event.target)) {
            searchResults.style.display = 'none';
        }
    });

    if (postalCodeVisibleInput && postalCodeVisibleInput.value.length === 5) {
        searchByPostalCode(postalCodeVisibleInput.value);
    }
});
