CREATE DATABASE crossword;

USE crossword;

CREATE TABLE bag_of_words (
    id int NOT NULL IDENTITY(1,1) PRIMARY KEY,
    x int NOT NULL,
	  y int NOT NULL,
    direction varchar(45) NOT NULL,
	  number int NOT NULL,
	  word varchar(255) NOT NULL,
	  clue varchar(255) NOT NULL
);