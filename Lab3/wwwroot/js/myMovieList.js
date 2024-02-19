const uri = 'api/movie';
let movieList = [];

function getMovies(username) {
    fetch(uri + "/user/" + username)
        .then(response => response.json())
        .then(data => _displayMovies(data))
        .catch(error => console.error('Unable to get items.', error));
}


function _displayMovies(data) {
    const tBody = document.getElementById('movieList');
    tBody.innerHTML = '';

    //_displayCount(data.length);


    data.forEach(movie => {
        let tr = tBody.insertRow();

        let td1 = tr.insertCell(0);
        let textNode = document.createTextNode(movie.title);
        td1.height = 200;
        td1.appendChild(textNode);

        //crud operations for movie
        let td2 = tr.insertCell(1);
        let editOption = document.createElement('a');
        editOption.innerHTML = 'Edit';
        editOption.href = 'movie/edit/' + movie.id;
        td2.appendChild(editOption);

        td2.appendChild(document.createTextNode(' | '));

        let deleteOption = document.createElement('a');
        deleteOption.innerHTML = 'Delete';
        deleteOption.method = 'delete';
        deleteOption.href = "/movie";
        td2.appendChild(deleteOption);

        deleteOption.addEventListener("click",function () {
            fetch(uri + '/' + movie.id, {
                method: 'DELETE'
            })
                .then(response => response.json())
                .then(() => {
                    getMovies();
                })
                .catch(error => console.error('Unable to delete item.', error));
        });

        let td3 = tr.insertCell(2);
        if (movie.imgUrl != null) {
            let movieEndPoint = 'movie/' + movie.id;
            let clickLink = document.createElement('a');
            clickLink.href = movieEndPoint;
            let img = document.createElement('img');
            img.src = movie.imgUrl;
            img.height = 200;
            clickLink.appendChild(img);
            td3.appendChild(clickLink);
        }

    });

    movieList = data;
}

