const uri = 'api/movie';
let movieList = [];

function getMovies() {
    fetch(uri)
        .then(response => response.json())
        .then(data => _displayMoviesInitial(data))
        .catch(error => console.error('Unable to get items.', error));
}


function _displayMoviesInitial(data) {
   

    //_displayCount(data.length);

    display(data)

    movieList = data;


    let searchTitle = ""

    searchTitle = document.getElementById('searchInput');
    let searchBtn = document.getElementById('searchButton');
    let genreFilter = document.getElementById('genreFilter');
    let yearFilter = document.getElementById('yearFilter');
    let rateFilter = document.getElementById('rateFilter');


    searchBtn.onclick = () => search(searchTitle);
    searchTitle.oninput = () => search(searchTitle)
    genreFilter.onchange = () => search(searchTitle);
    yearFilter.onchange = () => search(searchTitle);
    rateFilter.onchange = () => search(searchTitle);

}

function search (searchTitle) {
    if (searchTitle.value.trim() != null) {
        
        let searchValue = searchTitle.value;
        let searchKey = searchValue.split(' ');
        let filteredMovies = [];
        movieList.forEach(movie => {
            searchKey.forEach(key => {

                if (movie.title.toLowerCase().includes(key.toLowerCase())) {
                    if (!filteredMovies.includes(movie)) {
                        if (document.getElementById('genreFilter').value == movie.genre || document.getElementById('genreFilter').value == "All") {
                            if (document.getElementById('yearFilter').value == movie.releaseYear || document.getElementById('yearFilter').value == "All" || document.getElementById('yearFilter').value < 2000) {
                                if (document.getElementById('rateFilter').value <= movie.rating || document.getElementById('rateFilter').value == "All") {
                                    filteredMovies.push(movie);
                                }
                            }
                        }
                    }

                }
            })
        })

        display(filteredMovies);
    }
}

function display(data) {
    const tBody = document.getElementById('movieList');
    tBody.innerHTML = '';
    data.forEach(movie => {
        let tr = tBody.insertRow();

        let td1 = tr.insertCell(0);
        let textNode = document.createTextNode(movie.title);
        td1.height = 200;
        td1.appendChild(textNode);

        let td2 = tr.insertCell(1);
        let textNode1 = document.createTextNode(movie.genre);
        td2.appendChild(textNode1);

        let td4 = tr.insertCell(2);
        let textNode3 = document.createTextNode(movie.releaseYear);
        td4.appendChild(textNode3);

        let td5 = tr.insertCell(3);
        let textNode4 = document.createTextNode(movie.rating);
        td5.appendChild(textNode4);

        let td11 = tr.insertCell(4);
        if (movie.imgUrl != null) {
            let movieEndPoint = 'movie/' + movie.id;
            let clickLink = document.createElement('a');
            clickLink.href = movieEndPoint;
            let img = document.createElement('img');
            img.src = movie.imgUrl;
            img.height = 200;
            clickLink.appendChild(img);
            td11.appendChild(clickLink);
        }
    });
}


