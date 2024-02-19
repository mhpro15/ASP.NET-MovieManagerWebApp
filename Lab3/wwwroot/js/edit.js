const uri = '../../api/movie/';

function getMovie(id) {
    console.log(window.location.href)
    console.log(uri + id)
    fetch(uri + id)
        .then(response => response.json())
        .then(data => setMovie(data))
        .catch(error => console.error('Unable to get items.', error));


}

function setMovie(data) {
    document.getElementById('titleBox').value = data.title
    document.getElementById('genreBox').value = data.genre
    document.getElementById('yearBox').value = data.releaseYear
    document.getElementById('directorBox').value = data.director
    document.getElementById('imgBox').value = data.imgUrl
    console.log(data)
    let submitButton = document.getElementById('submitBtn');
    submitButton.addEventListener("click", function () {

        console.log("click")
        data.title = document.getElementById('titleBox').value;
        data.genre = document.getElementById('genreBox').value;
        data.releaseYear = Number(document.getElementById('yearBox').value);
        data.director = document.getElementById('directorBox').value;
        data.imgUrl = document.getElementById('imgBox').value;
        if (data.comments == null) {
            data.comments = [];
        }
        fetch(uri, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(data),
        })
            .then(response => response.json())
            .then(data => {
                console.log('Success:', data)
                getMovie(data.id);
            })
            .catch((error) => {
                console.error('Error:', error);
            });


    })


}