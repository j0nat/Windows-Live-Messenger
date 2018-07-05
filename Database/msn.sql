DROP TABLE IF EXISTS `account`;
CREATE TABLE IF NOT EXISTS `account` (
  `id` text NOT NULL,
  `name` text NOT NULL,
  `password` text NOT NULL,
  `comment` text NOT NULL,
  `avatar` text NOT NULL,
  `contacts` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `friend_requests`;
CREATE TABLE IF NOT EXISTS `friend_requests` (
  `requesterID` text NOT NULL,
  `targetID` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
COMMIT;