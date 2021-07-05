--IF (NOT EXISTS(SELECT * FROM MenubarItem WHERE ActionName = 'V3.ProcessFlowManager'))
--BEGIN
--INSERT INTO MenubarItem(Id, MenubarId, ParentId, Name, Header, ImageSource, ActionName, ToolTip, Seq, AccessLevel, StartUp, GroupId)
--VALUES ((SELECT MAX(Id) + 1 FROM MenubarItem), 1, (SELECT Id FROM MenubarItem WHERE Name = 'Setup'), 'ProcessFlowManager', 'Process Flow', NULL, 'V3.ProcessFlowManager', NULL, (SELECT MAX(Seq) + 1 FROM MenubarItem WHERE ActionName LIKE 'Setup%'), 100, NULL, NULL)
--END


IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'SuperNovaConnectionString'
          AND Object_ID = Object_ID(N'ConnectionProfile'))
BEGIN
    alter table ConnectionProfile add SuperNovaConnectionString varchar(150) null
END

