const uri = '../api/movie/';


function getMovie(id, username) {
    console.log(uri + id)
    fetch(uri + id)
        .then(response => response.json())
        .then(data => setMovie(data, username))
        .catch(error => console.error('Unable to get items.', error));
}


function setMovie(data, username) {
    let commentBlock = document.getElementById('commentBlock');
    let title = document.getElementById('movieTitle');
    title.innerHTML = data.title

    let rating = document.getElementById('myRange');
    let ratingValue = document.getElementById('rangeValue');
    let rateBtn = document.getElementById('rateMovie');
    ratingValue.innerHTML = data.rating;
    rating.onchange = function () {
        ratingValue.innerHTML = rating.value;
    }
    if (data.comments == null) {
        data.comments = [];
    }
    console.log(data)
    if (username != null && username != "") {
        rateBtn.onclick = function () {
            console.log(username)
            newRating = (Number(rating.value) + Number(data.rating) * Number(data.rateCount)) / (Number(data.rateCount) + 1);

            data.rating = newRating;
            data.rateCount = Number(data.rateCount) + 1;
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
                    ratingValue.innerHTML = data.rating;
                })
                .catch((error) => {
                    console.error('Error:', error);
                });

        }

        let newComment = document.createElement('textarea');
        newComment.id = 'newComment';
        let submitButton = document.createElement('button');

        submitButton.innerHTML = 'Submit';

        submitButton.onclick = function () {
            let currentDate = new Date();
            let comment = username + "||" + currentDate + "||" + newComment.value;
            data.comments.push(comment);
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
                    commentBlock.innerHTML = '';
                    getMovie(data.id, username);
                })
                .catch((error) => {
                    console.error('Error:', error);
                });
        }
        commentBlock.appendChild(document.createElement('br'))
        commentBlock.appendChild(newComment);
        commentBlock.appendChild(document.createElement('br'));
        commentBlock.appendChild(submitButton);
        commentBlock.appendChild(document.createElement('hr'));
    }

    data.comments.forEach((comment, index) => {
        //{"user1||2024-02-03 14:51:52||such a great movie!!"}
        let commentDiv = document.createElement('div');
        let commentInfo = comment.split('||');
        let user = commentInfo[0];
        let date = commentInfo[1];
        let text = commentInfo[2];

        if (commentInfo[2] != null) {
            let userComment = document.createElement('h4');
            userComment.innerHTML = user;
            commentDiv.appendChild(userComment);
            let commentDate = document.createElement('p');
            commentDate.innerHTML = date;
            commentDiv.appendChild(commentDate);
            let commentText = document.createElement('p');
            commentText.innerHTML = text;
            commentDiv.appendChild(commentText);


            commentBlock.appendChild(commentDiv);
            commentBlock.appendChild(document.createElement('hr'));
        }

        //allow to edit if less than 24 hours
        let cmtdate = new Date(commentInfo[1]);
        let currentDate = new Date();
        let timeDiff = currentDate - cmtdate;
        let hoursDiff = timeDiff / (1000 * 3600);

        //username = "user1"
        console.log(hoursDiff)
        console.log(hoursDiff < 24 && username == user && username != null)
        if (hoursDiff < 24 && username == user && username != null) {
            let editButton = document.createElement('button');
            editButton.innerHTML = 'Edit';
            editButton.onclick = function () {
                commentText = document.createElement('textarea');
                commentText.innerHTML = text;
                commentDiv.appendChild(commentText);
                commentDiv.removeChild(commentDiv.childNodes[2]);
                editButton.style.display = 'none';
                let saveButton = document.createElement('button');


                saveButton.innerHTML = 'Save';
                saveButton.onclick = function () {
                    let newComment = user + "||" + date + "||" + commentText.value;
                    console.log(newComment)
                    data.comments[index] = newComment;
                    commentDiv.removeChild(commentDiv.childNodes[3]);
                    commentText2 = document.createElement('p');
                    commentText2.innerHTML = commentText.value;
                    commentDiv.appendChild(commentText2);
                    console.log(data)
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

                        })
                        .catch((error) => {
                            console.error('Error:', error);
                        });
                    editButton.style.display = 'block';
                    saveButton.style.display = 'none';
                }
                commentDiv.appendChild(saveButton);
            }
            commentDiv.appendChild(editButton);
        }
    })


}