#默认超级管理员密码：123456
INSERT INTO `role` (id,name, permission_ids,create_id,ctime,mtime,is_delete) VALUES ('1', '超级管理员', '0', '0',NOW(),NOW(),'0');
INSERT INTO `user` (name,username,`password`,role_id,ctime,mtime,is_delete,is_system) VALUES ('超级管理员','admin','E10ADC3949BA59ABBE56E057F20F883E',1,NOW(),NOW(),0,1);