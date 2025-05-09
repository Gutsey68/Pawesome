document.addEventListener('DOMContentLoaded', async function() {
    const searchInput = document.getElementById('location-search');
    const searchResults = document.getElementById('search-results');
    let debounceTimer;
    let userLocation = null;
    let locationPromise;

    // Style CSS pour les résultats
    const style = document.createElement('style');
    style.textContent = `
        .search-result-item {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 12px 15px;
            cursor: pointer;
            border-bottom: 1px solid #eee;
        }
        
        .result-main {
            flex-grow: 1;
            margin-right: 15px;
        }
        
        .city-name {
            font-weight: 500;
            color: var(--color-primary-12);
        }
        
        .result-distance {
            color: var(--color-primary-12);
            font-size: 0.9em;
        }
        
        #search-results {
            -ms-overflow-style: none;
            scrollbar-width: none;
        }
    
        #search-results::-webkit-scrollbar {
            display: none;
        }

    `;
    document.head.appendChild(style);

    // Recherche des communes
    async function searchLocation(query) {
        if (query.length < 2) {
            searchResults.style.display = 'none';
            return;
        }
        
        try {
            const url = `https://geo.api.gouv.fr/communes?nom=${encodeURIComponent(query)}&boost=population&limit=10&fields=nom,code,centre`;
            const response = await fetch(url);
            const data = await response.json();
            displayResults(data);
        } catch (error) {
            console.error('Erreur lors de la recherche:', error);
            searchResults.style.display = 'none';
        }
    }

    // Affichage des résultats
    function displayResults(results) {
        searchResults.innerHTML = '';

        if (results.length === 0) {
            searchResults.style.display = 'none';
            return;
        }

        results.forEach(result => {
            const div = document.createElement('div');
            div.className = 'search-result-item';

            // On s'assure que la distance existe avant d'afficher le résultat
            if (result.distance !== null) {
                div.innerHTML = `
                <div class="result-main">
                    <div class="city-name">${result.nom}</div>
                </div>
            `;

                div.addEventListener('click', () => {
                    searchInput.value = result.nom;
                    searchResults.style.display = 'none';
                });

                searchResults.appendChild(div);
            }
        });

        if (searchResults.children.length > 0) {
            searchResults.style.display = 'block';
        } else {
            searchResults.style.display = 'none';
        }
    }

    // Écouteur d'événements avec debounce
    searchInput.addEventListener('input', function() {
        clearTimeout(debounceTimer);
        const query = this.value.trim();

        debounceTimer = setTimeout(() => {
            searchLocation(query);
        }, 500);
    });

    // Fermer les résultats en cliquant ailleurs
    document.addEventListener('click', function(event) {
        if (!searchInput.contains(event.target) && !searchResults.contains(event.target)) {
            searchResults.style.display = 'none';
        }
    });
});