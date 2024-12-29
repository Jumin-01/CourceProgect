/*
 Navicat Premium Dump SQL

 Source Server         : proj
 Source Server Type    : MySQL
 Source Server Version : 80040 (8.0.40)
 Source Host           : localhost:3306
 Source Schema         : home accounting

 Target Server Type    : MySQL
 Target Server Version : 80040 (8.0.40)
 File Encoding         : 65001

 Date: 29/12/2024 13:36:43
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for category
-- ----------------------------
DROP TABLE IF EXISTS `category`;
CREATE TABLE `category`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `ParentsCategory` int NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `ParentsCategory`(`ParentsCategory` ASC) USING BTREE,
  CONSTRAINT `ParentsCategory` FOREIGN KEY (`ParentsCategory`) REFERENCES `category` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 27 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of category
-- ----------------------------
INSERT INTO `category` VALUES (1, 'Витрати', NULL);
INSERT INTO `category` VALUES (2, 'Прибутки', NULL);
INSERT INTO `category` VALUES (3, 'М\'ясо', 10);
INSERT INTO `category` VALUES (4, 'Комуналка', 1);
INSERT INTO `category` VALUES (5, 'Вода', 4);
INSERT INTO `category` VALUES (6, 'Зарплатня', 2);
INSERT INTO `category` VALUES (7, 'Премія', 6);
INSERT INTO `category` VALUES (8, 'Риба', 10);
INSERT INTO `category` VALUES (10, 'Продукти', 1);
INSERT INTO `category` VALUES (20, 'Овочі', 10);
INSERT INTO `category` VALUES (21, 'Аванс', 2);
INSERT INTO `category` VALUES (23, 'Паливо', 4);
INSERT INTO `category` VALUES (24, 'Світло', 4);
INSERT INTO `category` VALUES (25, 'Відпочинок', 1);
INSERT INTO `category` VALUES (26, 'Шаурма', 10);

-- ----------------------------
-- Table structure for credit
-- ----------------------------
DROP TABLE IF EXISTS `credit`;
CREATE TABLE `credit`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` int NULL DEFAULT NULL,
  `RemainingAmount` decimal(10, 2) NULL DEFAULT NULL,
  `Amount` decimal(10, 2) NULL DEFAULT NULL,
  `Date` datetime NULL DEFAULT NULL,
  `UserName` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `UserToCredit`(`UserId` ASC) USING BTREE,
  CONSTRAINT `UserToCredit` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `chk_credit_amount` CHECK (`Amount` >= 1)
) ENGINE = InnoDB AUTO_INCREMENT = 8 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of credit
-- ----------------------------
INSERT INTO `credit` VALUES (1, 1, 2500.00, 4000.00, '2024-12-30 00:00:00', 'Іван');

-- ----------------------------
-- Table structure for creditpayment
-- ----------------------------
DROP TABLE IF EXISTS `creditpayment`;
CREATE TABLE `creditpayment`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `CreditId` int NULL DEFAULT NULL,
  `Amount` decimal(10, 2) NULL DEFAULT NULL,
  `Date` datetime NULL DEFAULT NULL,
  `UserName` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `CreditAmount` decimal(10, 2) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `idx_creditpayment_creditid`(`CreditId` ASC) USING BTREE,
  INDEX `idx_creditpayment_userid`(`UserName` ASC) USING BTREE,
  CONSTRAINT `Credit` FOREIGN KEY (`CreditId`) REFERENCES `credit` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `chk_creditpayment_amount` CHECK (`Amount` >= 1)
) ENGINE = InnoDB AUTO_INCREMENT = 9 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of creditpayment
-- ----------------------------
INSERT INTO `creditpayment` VALUES (3, 1, 1500.00, '2024-12-27 00:00:00', NULL, NULL);

-- ----------------------------
-- Table structure for plan
-- ----------------------------
DROP TABLE IF EXISTS `plan`;
CREATE TABLE `plan`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` int NULL DEFAULT NULL,
  `Type` enum('income','expense') CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `Amount` decimal(10, 2) NULL DEFAULT NULL,
  `CategoryId` int NULL DEFAULT NULL,
  `Period` date NULL DEFAULT NULL,
  `CategoryName` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `UserName` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `idx_plan_userid`(`UserId` ASC) USING BTREE,
  INDEX `PlanCategory`(`CategoryId` ASC) USING BTREE,
  CONSTRAINT `PlanCategory` FOREIGN KEY (`CategoryId`) REFERENCES `category` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `UserToPlan` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `chk_plan_amount` CHECK (`Amount` >= 1)
) ENGINE = InnoDB AUTO_INCREMENT = 13 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of plan
-- ----------------------------
INSERT INTO `plan` VALUES (7, 4, 'expense', 1000.00, 5, '2024-12-25', 'Вода', 'Кирило');
INSERT INTO `plan` VALUES (8, 1, 'income', 400.00, 3, '2025-01-01', 'М\'ясо', 'Іван');
INSERT INTO `plan` VALUES (10, 2, 'expense', 200.00, 10, '2024-12-28', 'Продукти', 'Веніамін');

-- ----------------------------
-- Table structure for transaction
-- ----------------------------
DROP TABLE IF EXISTS `transaction`;
CREATE TABLE `transaction`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` int NULL DEFAULT NULL,
  `CategoryId` int NULL DEFAULT NULL,
  `Type` enum('income','expense') CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `Amount` decimal(10, 2) NULL DEFAULT NULL,
  `Date` datetime NULL DEFAULT NULL,
  `CategoryName` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `UserName` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `idx_transaction_userid`(`UserId` ASC) USING BTREE,
  INDEX `UserCategory`(`CategoryId` ASC) USING BTREE,
  CONSTRAINT `UserCategory` FOREIGN KEY (`CategoryId`) REFERENCES `category` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `UserToTransaction` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `chk_transaction_amount` CHECK (`Amount` >= 1)
) ENGINE = InnoDB AUTO_INCREMENT = 22 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of transaction
-- ----------------------------
INSERT INTO `transaction` VALUES (10, 1, 3, 'income', 200.00, '2024-12-28 00:00:00', 'М\'ясо', 'Іван');
INSERT INTO `transaction` VALUES (11, 1, 7, 'income', 600.00, '2024-12-27 00:00:00', 'Премія', 'Іван');
INSERT INTO `transaction` VALUES (12, 1, 5, 'expense', 100.00, '2024-12-27 00:00:00', 'Вода', 'Іван');
INSERT INTO `transaction` VALUES (13, 1, 6, 'income', 1000.00, '2025-01-05 00:00:00', 'Зарплатня', 'Іван');
INSERT INTO `transaction` VALUES (14, 1, 25, 'expense', 200.00, '2025-01-05 00:00:00', 'Відпочинок', 'Іван');
INSERT INTO `transaction` VALUES (15, 1, 8, 'expense', 200.00, '2024-12-29 00:00:00', 'Риба', 'Іван');
INSERT INTO `transaction` VALUES (16, 2, 6, 'income', 4000.00, '2024-12-27 00:00:00', 'Зарплатня', 'Веніамін');
INSERT INTO `transaction` VALUES (17, 2, 25, 'expense', 200.00, '2024-12-27 00:00:00', 'Відпочинок', 'Веніамін');
INSERT INTO `transaction` VALUES (18, 2, 5, 'income', 200.00, '2024-12-29 00:00:00', 'Вода', 'Веніамін');
INSERT INTO `transaction` VALUES (19, 1, 21, 'income', 4000.00, '2024-12-28 00:00:00', 'Аванс', 'Іван');
INSERT INTO `transaction` VALUES (21, 1, 21, 'income', 10000.00, '2024-12-24 00:00:00', 'Аванс', 'Іван');

-- ----------------------------
-- Table structure for users
-- ----------------------------
DROP TABLE IF EXISTS `users`;
CREATE TABLE `users`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Password` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NULL DEFAULT NULL,
  `Role` enum('Parents','Children') CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `Balance` decimal(10, 2) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`, `Name`) USING BTREE,
  INDEX `Id`(`Id` ASC) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 17 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of users
-- ----------------------------
INSERT INTO `users` VALUES (1, 'Іван', '123', 'Parents', 14155.00);
INSERT INTO `users` VALUES (2, 'Веніамін', '321', 'Children', 4139.99);
INSERT INTO `users` VALUES (3, 'Марія', '4435увкпа', 'Children', 829.02);
INSERT INTO `users` VALUES (4, 'Кирило', 'віапуке', 'Children', 120.32);
INSERT INTO `users` VALUES (5, 'Єлизавета', 'вчапівап', 'Children', 13.55);
INSERT INTO `users` VALUES (7, 'Тимофій', 'сапівап', 'Parents', 272.81);
INSERT INTO `users` VALUES (8, 'Дмитрій', 'авіа', 'Parents', 802.47);

-- ----------------------------
-- View structure for carview
-- ----------------------------
DROP VIEW IF EXISTS `carview`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `carview` AS select `models`.`Name` AS `modelname`,`brands`.`Name` AS `bramdName`,`clients`.`address` AS `address`,`persons`.`FullName` AS `FullName`,`persons`.`email` AS `email`,`persons`.`phone` AS `phone` from ((((`car` join `models` on((`car`.`ModelId` = `models`.`Id`))) join `brands` on((`models`.`brandId` = `brands`.`Id`))) join `clients` on((`car`.`ClientId` = `clients`.`Id`))) join `persons` on((`clients`.`personId` = `persons`.`Id`)));

-- ----------------------------
-- View structure for usersummary
-- ----------------------------
DROP VIEW IF EXISTS `usersummary`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `usersummary` AS select `u`.`Id` AS `UserId`,`u`.`Name` AS `UserName`,`u`.`Balance` AS `UserBalance`,count((case when (`t`.`Type` = 'income') then 1 else NULL end)) AS `IncomeCount`,sum((case when (`t`.`Type` = 'income') then `t`.`Amount` else 0 end)) AS `TotalIncome`,count((case when (`t`.`Type` = 'expense') then 1 else NULL end)) AS `ExpenseCount`,sum((case when (`t`.`Type` = 'expense') then `t`.`Amount` else 0 end)) AS `TotalExpense`,count(distinct `c`.`Id`) AS `CreditCount`,sum(`c`.`Amount`) AS `TotalCreditAmount`,sum(`c`.`RemainingAmount`) AS `TotalRemainingDebt` from ((`users` `u` left join `transaction` `t` on((`u`.`Id` = `t`.`UserId`))) left join `credit` `c` on((`u`.`Id` = `c`.`UserId`))) group by `u`.`Id`,`u`.`Name`,`u`.`Balance`;

-- ----------------------------
-- Procedure structure for AddCredit
-- ----------------------------
DROP PROCEDURE IF EXISTS `AddCredit`;
delimiter ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `AddCredit`(
    IN p_UserId INT,
    IN p_Amount DECIMAL(10, 2),
    IN p_Date DATETIME
)
BEGIN
    DECLARE user_name VARCHAR(255);

    -- Отримання імені користувача
    SELECT Name INTO user_name
    FROM users
    WHERE Id = p_UserId;

    -- Вставка нового кредиту з іменем користувача
    INSERT INTO credit (UserId, UserName, Amount, RemainingAmount, Date)
    VALUES (p_UserId, user_name, p_Amount, p_Amount, p_Date);

    -- Оновлення балансу користувача
    UPDATE users
    SET Balance = Balance + p_Amount
    WHERE Id = p_UserId;
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for AddCreditPayment
-- ----------------------------
DROP PROCEDURE IF EXISTS `AddCreditPayment`;
delimiter ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `AddCreditPayment`(
    IN p_CreditId INT,
    IN p_Amount DECIMAL(10, 2),
    IN p_Date DATETIME
)
BEGIN
    DECLARE remaining_amount DECIMAL(10, 2);

    -- Отримання залишкової суми кредиту
    SELECT RemainingAmount INTO remaining_amount
    FROM credit
    WHERE Id = p_CreditId;

    -- Перевірка, чи не перевищує сума виплати залишкову суму
    IF p_Amount > remaining_amount THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Payment amount exceeds remaining credit amount';
    END IF;

    -- Вставка нового платежу
    INSERT INTO creditpayment (CreditId, Amount, Date)
    VALUES (p_CreditId, p_Amount, p_Date);

    -- Оновлення залишкової суми кредиту
    
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for AddPlan
-- ----------------------------
DROP PROCEDURE IF EXISTS `AddPlan`;
delimiter ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `AddPlan`(
    IN p_UserId INT,
    IN p_CategoryId INT,
    IN p_Type ENUM('income', 'expense'),
    IN p_Amount DECIMAL(10, 2),
    IN p_Period DATE
)
BEGIN
    DECLARE category_name VARCHAR(255);
    DECLARE user_name VARCHAR(255);

    -- Отримання імені категорії
    SELECT name INTO category_name
    FROM category
    WHERE Id = p_CategoryId;

    -- Отримання імені користувача
    SELECT name INTO user_name
    FROM users
    WHERE Id = p_UserId;

    -- Вставка даних у таблицю plan
    INSERT INTO plan (UserId, CategoryId, Type, Amount, Period, CategoryName, UserName)
    VALUES (p_UserId, p_CategoryId, p_Type, p_Amount, p_Period, category_name, user_name);
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for AddTransaction
-- ----------------------------
DROP PROCEDURE IF EXISTS `AddTransaction`;
delimiter ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `AddTransaction`(
    IN p_UserId INT,
    IN p_CategoryId INT,
    IN p_Type ENUM('income', 'expense'),
    IN p_Amount DECIMAL(10, 2),
    IN p_Date DATETIME
)
BEGIN
    DECLARE category_name VARCHAR(255);
    DECLARE user_name VARCHAR(255);

    -- Отримання імені категорії
    SELECT name INTO category_name
    FROM category
    WHERE Id = p_CategoryId;

    -- Отримання імені користувача
    SELECT name INTO user_name
    FROM users
    WHERE Id = p_UserId;

    -- Вставка даних у таблицю transaction
    INSERT INTO transaction (UserId, CategoryId, Type, Amount, Date, CategoryName, UserName)
    VALUES (p_UserId, p_CategoryId, p_Type, p_Amount, p_Date, category_name, user_name);
END
;;
delimiter ;

-- ----------------------------
-- Function structure for GetTotalTransactions
-- ----------------------------
DROP FUNCTION IF EXISTS `GetTotalTransactions`;
delimiter ;;
CREATE DEFINER=`root`@`localhost` FUNCTION `GetTotalTransactions`(
    p_UserId INT,
    p_Type ENUM('income', 'expense')
) RETURNS decimal(10,2)
    DETERMINISTIC
BEGIN
    DECLARE total_amount DECIMAL(10, 2);

    SELECT SUM(Amount) INTO total_amount
    FROM transaction
    WHERE UserId = p_UserId AND Type = p_Type;

    RETURN IFNULL(total_amount, 0);
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for password_decrypt
-- ----------------------------
DROP PROCEDURE IF EXISTS `password_decrypt`;
delimiter ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `password_decrypt`(IN enc_key VARCHAR(20))
BEGIN
    UPDATE users
    SET password = CAST(AES_DECRYPT(UNHEX(password), enc_key) AS CHAR);
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for password_encrypt
-- ----------------------------
DROP PROCEDURE IF EXISTS `password_encrypt`;
delimiter ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `password_encrypt`(IN enc_key VARCHAR(20))
BEGIN
    UPDATE users
    SET password = HEX(AES_ENCRYPT(password, enc_key));
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for pwd_decrypt
-- ----------------------------
DROP PROCEDURE IF EXISTS `pwd_decrypt`;
delimiter ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `pwd_decrypt`(IN enc_key varchar(20))
BEGIN
  update users set `password`=AES_DECRYPT(UNHEX(`password`), enc_key);
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for pwd_enc
-- ----------------------------
DROP PROCEDURE IF EXISTS `pwd_enc`;
delimiter ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `pwd_enc`(IN enc_key varchar(20))
BEGIN
  update users set `password`=HEX(AES_ENCRYPT(`password`, ecn_key ));
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for pwd_encrypt
-- ----------------------------
DROP PROCEDURE IF EXISTS `pwd_encrypt`;
delimiter ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `pwd_encrypt`(IN ecn_key varchar(20))
BEGIN
  update users set `password`=HEX(AES_ENCRYPT(`password`, ecn_key ));
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for UpdateCredit
-- ----------------------------
DROP PROCEDURE IF EXISTS `UpdateCredit`;
delimiter ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `UpdateCredit`(
    IN p_CreditId INT,
    IN p_UserId INT,
    IN p_Amount DECIMAL(10, 2),
    IN p_RemainingAmount DECIMAL(10, 2),
    IN p_Date DATETIME
)
BEGIN
    DECLARE old_amount DECIMAL(10, 2);

    -- Отримання старої суми кредиту
    SELECT Amount INTO old_amount
    FROM credit
    WHERE Id = p_CreditId;

    -- Перевірка, чи залишкова сума не перевищує загальну суму
    IF p_RemainingAmount > p_Amount THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Remaining amount cannot exceed total amount';
    END IF;

    -- Оновлення кредиту
    UPDATE credit
    SET 
        UserId = p_UserId,
        Amount = p_Amount,
        RemainingAmount = p_RemainingAmount,
        Date = p_Date
    WHERE Id = p_CreditId;

    -- Оновлення балансу користувача
    UPDATE users
    SET Balance = Balance - old_amount + p_Amount
    WHERE Id = p_UserId;
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for UpdateCreditPayment
-- ----------------------------
DROP PROCEDURE IF EXISTS `UpdateCreditPayment`;
delimiter ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `UpdateCreditPayment`(
    IN p_PaymentId INT,
    IN p_CreditId INT,
    IN p_Amount DECIMAL(10, 2),
    IN p_Date DATETIME
)
BEGIN
    DECLARE old_amount DECIMAL(10, 2);
    DECLARE remaining_amount DECIMAL(10, 2);

    -- Отримання старої суми платежу
    SELECT Amount INTO old_amount
    FROM creditpayment
    WHERE Id = p_PaymentId;

    -- Отримання залишкової суми кредиту
    SELECT RemainingAmount INTO remaining_amount
    FROM credit
    WHERE Id = p_CreditId;

    -- Перевірка, чи не перевищує нова сума виплати залишкову суму
    IF (remaining_amount + old_amount) < p_Amount THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Updated payment amount exceeds remaining credit amount';
    END IF;

    -- Оновлення платежу
    UPDATE creditpayment
    SET 
        CreditId = p_CreditId,
        Amount = p_Amount,
        Date = p_Date
    WHERE Id = p_PaymentId;

    -- Оновлення залишкової суми кредиту
    UPDATE credit
    SET RemainingAmount = RemainingAmount + old_amount - p_Amount
    WHERE Id = p_CreditId;
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for UpdatePlan
-- ----------------------------
DROP PROCEDURE IF EXISTS `UpdatePlan`;
delimiter ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `UpdatePlan`(
    IN p_PlanId INT,
    IN p_UserId INT,
    IN p_CategoryId INT,
    IN p_Type ENUM('income', 'expense'),
    IN p_Amount DECIMAL(10, 2),
    IN p_Period DATE
)
BEGIN
    DECLARE category_name VARCHAR(255);
    DECLARE user_name VARCHAR(255);

    -- Отримання імені категорії
    SELECT name INTO category_name
    FROM category
    WHERE Id = p_CategoryId;

    -- Отримання імені користувача
    SELECT name INTO user_name
    FROM users
    WHERE Id = p_UserId;

    -- Оновлення даних у таблиці plan
    UPDATE plan
    SET 
        UserId = p_UserId,
        CategoryId = p_CategoryId,
        Type = p_Type,
        Amount = p_Amount,
        Period = p_Period,
        CategoryName = category_name,
        UserName = user_name
    WHERE Id = p_PlanId;
END
;;
delimiter ;

-- ----------------------------
-- Procedure structure for UpdateTransaction
-- ----------------------------
DROP PROCEDURE IF EXISTS `UpdateTransaction`;
delimiter ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `UpdateTransaction`(
    IN p_TransactionId INT,
    IN p_UserId INT,
    IN p_CategoryId INT,
    IN p_Type ENUM('income', 'expense'),
    IN p_Amount DECIMAL(10, 2),
    IN p_Date DATETIME
)
BEGIN
    DECLARE category_name VARCHAR(255);
    DECLARE user_name VARCHAR(255);

    -- Отримання імені категорії
    SELECT name INTO category_name
    FROM category
    WHERE Id = p_CategoryId;

    -- Отримання імені користувача
    SELECT name INTO user_name
    FROM users
    WHERE Id = p_UserId;

    -- Оновлення даних у таблиці transaction
    UPDATE transaction
    SET 
        UserId = p_UserId,
        CategoryId = p_CategoryId,
        Type = p_Type,
        Amount = p_Amount,
        Date = p_Date,
        CategoryName = category_name,
        UserName = user_name
    WHERE Id = p_TransactionId;
END
;;
delimiter ;

-- ----------------------------
-- Triggers structure for table credit
-- ----------------------------
DROP TRIGGER IF EXISTS `trg_after_insert_credit`;
delimiter ;;
CREATE TRIGGER `trg_after_insert_credit` AFTER INSERT ON `credit` FOR EACH ROW BEGIN
    UPDATE users
    SET Balance = Balance + NEW.Amount
    WHERE Id = NEW.UserId;
END
;;
delimiter ;

-- ----------------------------
-- Triggers structure for table credit
-- ----------------------------
DROP TRIGGER IF EXISTS `trg_after_delete_credit`;
delimiter ;;
CREATE TRIGGER `trg_after_delete_credit` AFTER DELETE ON `credit` FOR EACH ROW BEGIN
    -- Повернення балансу користувача при видаленні кредиту
    UPDATE users
    SET Balance = Balance - OLD.Amount
    WHERE Id = OLD.UserId;
END
;;
delimiter ;

-- ----------------------------
-- Triggers structure for table creditpayment
-- ----------------------------
DROP TRIGGER IF EXISTS `trg_creditpayment_amount_check`;
delimiter ;;
CREATE TRIGGER `trg_creditpayment_amount_check` BEFORE INSERT ON `creditpayment` FOR EACH ROW BEGIN
    DECLARE remaining_amount DECIMAL(10, 2);

    -- Отримання залишкової суми кредиту
    SELECT RemainingAmount INTO remaining_amount
    FROM credit
    WHERE Id = NEW.CreditId;

    -- Перевірка, чи не перевищує сума виплати залишкову суму кредиту
    IF NEW.Amount > remaining_amount THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Amount in creditpayment cannot exceed the RemainingAmount of the related credit';
    END IF;

END
;;
delimiter ;

-- ----------------------------
-- Triggers structure for table creditpayment
-- ----------------------------
DROP TRIGGER IF EXISTS `trg_amoungupdate_insert_creditpayment`;
delimiter ;;
CREATE TRIGGER `trg_amoungupdate_insert_creditpayment` AFTER INSERT ON `creditpayment` FOR EACH ROW BEGIN
    DECLARE total_paid DECIMAL(10, 2);

    -- Розрахунок загальної суми платежів за кредит
    SELECT SUM(Amount) INTO total_paid
    FROM creditpayment
    WHERE CreditId = NEW.CreditId;

    -- Оновлення залишкової суми в таблиці credit
    UPDATE credit
    SET RemainingAmount = Amount - total_paid
    WHERE Id = NEW.CreditId;
END
;;
delimiter ;

-- ----------------------------
-- Triggers structure for table creditpayment
-- ----------------------------
DROP TRIGGER IF EXISTS `trg_after_insert_creditpayment`;
delimiter ;;
CREATE TRIGGER `trg_after_insert_creditpayment` AFTER INSERT ON `creditpayment` FOR EACH ROW BEGIN
   
    UPDATE users
    SET Balance = Balance - NEW.Amount
    WHERE Id = (SELECT UserId FROM credit WHERE Id = NEW.CreditId);
END
;;
delimiter ;

-- ----------------------------
-- Triggers structure for table creditpayment
-- ----------------------------
DROP TRIGGER IF EXISTS `trg_amoungupdate_update_creditpayment`;
delimiter ;;
CREATE TRIGGER `trg_amoungupdate_update_creditpayment` AFTER UPDATE ON `creditpayment` FOR EACH ROW BEGIN
    DECLARE total_paid DECIMAL(10, 2);

    -- Розрахунок загальної суми платежів за кредит
    SELECT SUM(Amount) INTO total_paid
    FROM creditpayment
    WHERE CreditId = NEW.CreditId;

    -- Оновлення залишкової суми в таблиці credit
    UPDATE credit
    SET RemainingAmount = Amount - total_paid
    WHERE Id = NEW.CreditId;
END
;;
delimiter ;

-- ----------------------------
-- Triggers structure for table creditpayment
-- ----------------------------
DROP TRIGGER IF EXISTS `trg_after_update_creditpayment`;
delimiter ;;
CREATE TRIGGER `trg_after_update_creditpayment` AFTER UPDATE ON `creditpayment` FOR EACH ROW BEGIN
    UPDATE users
    SET Balance = Balance + OLD.Amount - NEW.Amount
    WHERE Id = (SELECT UserId FROM credit WHERE Id = NEW.CreditId);
END
;;
delimiter ;

-- ----------------------------
-- Triggers structure for table creditpayment
-- ----------------------------
DROP TRIGGER IF EXISTS `trg_after_delete_creditpayment`;
delimiter ;;
CREATE TRIGGER `trg_after_delete_creditpayment` AFTER DELETE ON `creditpayment` FOR EACH ROW BEGIN
    DECLARE user_id INT;

    -- Отримання UserId з таблиці credit
    SELECT UserId INTO user_id
    FROM credit
    WHERE Id = OLD.CreditId;

    -- Оновлення балансу користувача після видалення платежу
    UPDATE users
    SET Balance = Balance + OLD.Amount
    WHERE Id = user_id;

    -- Оновлення залишкової суми кредиту після видалення платежу
    UPDATE credit
    SET RemainingAmount = RemainingAmount + OLD.Amount
    WHERE Id = OLD.CreditId;
END
;;
delimiter ;

-- ----------------------------
-- Triggers structure for table transaction
-- ----------------------------
DROP TRIGGER IF EXISTS `trg_after_insert_transaction`;
delimiter ;;
CREATE TRIGGER `trg_after_insert_transaction` AFTER INSERT ON `transaction` FOR EACH ROW BEGIN
    
   
    IF NEW.Type = 'income' THEN
        UPDATE users
        SET Balance = Balance + NEW.Amount
        WHERE Id = NEW.UserId;
    ELSEIF NEW.Type = 'expense' THEN
        UPDATE users
        SET Balance = Balance - NEW.Amount
        WHERE Id = NEW.UserId;
    END IF;
END
;;
delimiter ;

-- ----------------------------
-- Triggers structure for table transaction
-- ----------------------------
DROP TRIGGER IF EXISTS `trg_after_update_transaction`;
delimiter ;;
CREATE TRIGGER `trg_after_update_transaction` AFTER UPDATE ON `transaction` FOR EACH ROW BEGIN
   
    IF OLD.Type = 'income' THEN
        UPDATE users
        SET Balance = Balance - OLD.Amount
        WHERE Id = OLD.UserId;
    ELSEIF OLD.Type = 'expense' THEN
        UPDATE users
        SET Balance = Balance + OLD.Amount
        WHERE Id = OLD.UserId;
    END IF;

    IF NEW.Type = 'income' THEN
        UPDATE users
        SET Balance = Balance + NEW.Amount
        WHERE Id = NEW.UserId;
    ELSEIF NEW.Type = 'expense' THEN
        UPDATE users
        SET Balance = Balance - NEW.Amount
        WHERE Id = NEW.UserId;
    END IF;
END
;;
delimiter ;

-- ----------------------------
-- Triggers structure for table transaction
-- ----------------------------
DROP TRIGGER IF EXISTS `trg_after_delete_transaction`;
delimiter ;;
CREATE TRIGGER `trg_after_delete_transaction` BEFORE DELETE ON `transaction` FOR EACH ROW BEGIN
    IF OLD.Type = 'income' THEN
        UPDATE users
        SET Balance = Balance - OLD.Amount
        WHERE Id = OLD.UserId;
    ELSEIF OLD.Type = 'expense' THEN
        UPDATE users
        SET Balance = Balance + OLD.Amount
        WHERE Id = OLD.UserId;
    END IF;
END
;;
delimiter ;

SET FOREIGN_KEY_CHECKS = 1;
