/*
 Navicat Premium Data Transfer

 Source Server         : 127.0.0.1
 Source Server Type    : MySQL
 Source Server Version : 50562
 Source Host           : localhost:3306
 Source Schema         : userintegraldb

 Target Server Type    : MySQL
 Target Server Version : 50562
 File Encoding         : 65001

 Date: 31/10/2018 10:03:14
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for integral_info
-- ----------------------------
DROP TABLE IF EXISTS `integral_info`;
CREATE TABLE `integral_info`  (
  `userID` varchar(255) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `integral` double(255, 0) NULL DEFAULT NULL,
  `coupons` double(255, 0) NULL DEFAULT NULL,
  `roomCard` int(255) NULL DEFAULT NULL,
  PRIMARY KEY (`userID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_bin ROW_FORMAT = Compact;

-- ----------------------------
-- Table structure for user_operation_log
-- ----------------------------
DROP TABLE IF EXISTS `user_operation_log`;
CREATE TABLE `user_operation_log`  (
  `userID` varchar(255) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `business` varchar(255) CHARACTER SET utf8 COLLATE utf8_bin NULL DEFAULT NULL,
  `action` varchar(255) CHARACTER SET utf8 COLLATE utf8_bin NULL DEFAULT NULL,
  `type` varchar(255) CHARACTER SET utf8 COLLATE utf8_bin NULL DEFAULT NULL,
  `count` double NULL DEFAULT NULL,
  `dateTime` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`userID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_bin ROW_FORMAT = Compact;

-- ----------------------------
-- Table structure for users
-- ----------------------------
DROP TABLE IF EXISTS `users`;
CREATE TABLE `users`  (
  `UID` varchar(255) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `InOut` int(255) NULL DEFAULT NULL,
  `PayType` int(255) NULL DEFAULT NULL,
  `FromTo` int(255) NULL DEFAULT NULL,
  `Val` int(255) NULL DEFAULT NULL,
  `Remark` varchar(255) CHARACTER SET utf8 COLLATE utf8_bin NULL DEFAULT NULL,
  `Status` int(255) NULL DEFAULT NULL,
  `CreateTime` datetime NULL DEFAULT NULL,
  `UpdateTime` datetime NULL DEFAULT NULL,
  `PriceType` int(255) NULL DEFAULT NULL,
  PRIMARY KEY (`UID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_bin ROW_FORMAT = Compact;

SET FOREIGN_KEY_CHECKS = 1;
