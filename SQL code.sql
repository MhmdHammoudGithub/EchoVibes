CREATE DATABASE echovibe;

USE echovibe;

CREATE TABLE _user(
    user_id INT AUTO_INCREMENT PRIMARY KEY,
	full_name VARCHAR(50) NOT NULL COLLATE utf8_bin,
    password VARCHAR(1024) NOT NULL COLLATE utf8_bin,
    date_of_birth DATE NOT NULL,
    gender VARCHAR(1) NOT NULL
);

CREATE TABLE _post(
    post_id INT AUTO_INCREMENT PRIMARY KEY,
    poster_id INT NOT NULL,
    content VARCHAR(1000) NOT NULL COLLATE utf8_bin,
    date_of_post DATETIME NOT NULL,
    like_counter INT DEFAULT 0,
    comment_counter INT DEFAULT 0,
    FOREIGN KEY (poster_id) REFERENCES _user(user_id) ON DELETE CASCADE
);

CREATE TABLE _comment(
    comment_id INT AUTO_INCREMENT PRIMARY KEY,
    post_id INT NOT NULL,
    commentator_id INT NOT NULL,
    content VARCHAR(1000) NOT NULL COLLATE utf8_bin,
    date_of_comment DATETIME NOT NULL,
    like_counter INT DEFAULT 0,
    FOREIGN KEY (post_id) REFERENCES _post(post_id) ON DELETE CASCADE,
    FOREIGN KEY (commentator_id) REFERENCES _user(user_id) ON DELETE CASCADE
);

CREATE TABLE _post_like (
    like_id INT AUTO_INCREMENT PRIMARY KEY,
    post_id INT NOT NULL,
    liker_id INT NOT NULL,
    FOREIGN KEY (post_id) REFERENCES _post(post_id) ON DELETE CASCADE,
    FOREIGN KEY (liker_id) REFERENCES _user(user_id) ON DELETE CASCADE
);

CREATE TABLE _comment_like (
    like_id INT AUTO_INCREMENT PRIMARY KEY,
    comment_id INT NOT NULL,
    liker_id INT NOT NULL,
    FOREIGN KEY (comment_id) REFERENCES _comment(comment_id) ON DELETE CASCADE,
    FOREIGN KEY (liker_id) REFERENCES _user(user_id) ON DELETE CASCADE
);

CREATE TABLE _friend(
    friend_id INT AUTO_INCREMENT PRIMARY KEY,
    user1_id INT NOT NULL,
    user2_id INT NOT NULL,
    status VARCHAR(10) DEFAULT "pending",
    FOREIGN KEY (user1_id) REFERENCES _user(user_id) ON DELETE CASCADE,
    FOREIGN KEY (user2_id) REFERENCES _user(user_id) ON DELETE CASCADE
);














