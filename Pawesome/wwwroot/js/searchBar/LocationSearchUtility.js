document.addEventListener('DOMContentLoaded', async function() {
    const searchInput = document.getElementById('location-search');
    const searchResults = document.getElementById('search-results');
    let debounceTimer;
    let userLocation = null;
    let locationPromise;

    // Attendez que la géolocalisation soit initialisée
    try {
        initializeGeolocation();
    } catch (error) {
        console.error("Impossible d'obtenir la géolocalisation:", error);
    }

    // Le reste du code...
    // Obtenir la géolocalisation
    if ("geolocation" in navigator) {
        navigator.geolocation.getCurrentPosition(
            function(position) {
                userLocation = {
                    lat: position.coords.latitude,
                    lon: position.coords.longitude
                };
            },
            function(error) {
                console.error("Erreur de géolocalisation:", error);
            }
        );
    }

    // Calcul de distance
    function getDistance(lat1, lon1, lat2, lon2) {
        const R = 6371;
        const dLat = (lat2 - lat1) * Math.PI / 180;
        const dLon = (lon2 - lon1) * Math.PI / 180;
        const a = Math.sin(dLat/2) * Math.sin(dLat/2) +
            Math.cos(lat1 * Math.PI / 180) * Math.cos(lat2 * Math.PI / 180) *
            Math.sin(dLon/2) * Math.sin(dLon/2);
        const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1-a));
        return R * c;
    }

    function formatDistance(distance) {
        if (distance < 1) {
            return `${Math.round(distance * 1000)} m`;
        }
        return `${Math.round(distance * 10) / 10} km`;
    }

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
            // Attendre la géolocalisation
            await locationPromise;

            const url = `https://geo.api.gouv.fr/communes?nom=${encodeURIComponent(query)}&boost=population&limit=10&fields=nom,code,centre`;
            const response = await fetch(url);
            const data = await response.json();

            // Calcul des distances...
            const resultsPromises = data.map(async commune => {
                if (!commune.centre || !userLocation) {
                    return { ...commune, distance: null };
                }

                const distance = await new Promise(resolve => {
                    setTimeout(() => {
                        const dist = getDistance(
                            userLocation.lat,
                            userLocation.lon,
                            commune.centre.coordinates[1],
                            commune.centre.coordinates[0]
                        );
                        resolve(dist);
                    }, 100);
                });

                return {
                    ...commune,
                    distance
                };
            });

            const results = await Promise.all(resultsPromises);
            const sortedResults = results.sort((a, b) => {
                if (a.distance !== null && b.distance !== null) {
                    return a.distance - b.distance;
                }
                return 0;
            });

            displayResults(sortedResults);
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
                <div class="result-distance">${formatDistance(result.distance)}</div>
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

// Modifiez la partie de géolocalisation comme ceci :
function initializeGeolocation() {
    return new Promise((resolve, reject) => {
        if ("geolocation" in navigator) {
            navigator.geolocation.getCurrentPosition(
                function(position) {
                    userLocation = {
                        lat: position.coords.latitude,
                        lon: position.coords.longitude
                    };
                    resolve(userLocation);
                },
                function(error) {
                    console.error("Erreur de géolocalisation:", error);
                    reject(error);
                }
            );
        } else {
            reject("Géolocalisation non supportée");
        }
    });
}