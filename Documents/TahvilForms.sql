USE [PCStock]
GO

/****** Object:  View [dbo].[TahvilForms]    Script Date: 2/5/2020 11:12:23 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO










ALTER VIEW [dbo].[TahvilForms] 
AS
SELECT
  dbo.TahvilHeaderTBL.CodeTahvil,
  dbo.TahvilHeaderTBL.TypeTahvil,
  dbo.TahvilHeaderTBL.DateTahvil,
  dbo.TahvilHeaderTBL.DeliverCode AS DevilerCodeOrigin,
  CASE TypeTahvil
    WHEN 1 THEN N'رايانه'
    WHEN 2 THEN N'تجهيزات جانبي'
    WHEN 3 THEN N'لوازم مصرفي'
    WHEN 4 THEN 'باطل شده'
  END AS TypeName,
  (SELECT
    Firstname + ' ' + Lastname AS Expr1
  FROM dbo.PersonTBL
  WHERE (Row = dbo.TahvilHeaderTBL.DeliverCode))
  AS DeliverCode,
  (SELECT
    keypersenel
  FROM dbo.PersonTBL
  WHERE (Row = dbo.TahvilHeaderTBL.DeliverCode))
  AS RegisterNo,
  (SELECT
    MemberName
  FROM dbo.MemberTBL
  WHERE (MemberCode = dbo.TahvilHeaderTBL.ApproveCode))
  AS ApproveCode,
  (SELECT
    OfficeName
  FROM dbo.OfficeTBL
  WHERE (KeyOffice IN (SELECT
    KeyOffice
  FROM dbo.PersonTBL
  WHERE (Row = dbo.TahvilHeaderTBL.DeliverCode))
  ))
  AS Office,
  (SELECT
    Managmentname
  FROM dbo.ManagmentTBL
  WHERE (KeyName IN (SELECT
    Managmentkey
  FROM dbo.PersonTBL
  WHERE (Row = dbo.TahvilHeaderTBL.DeliverCode))
  ))
  AS Managment,
  dbo.TahvilHeaderTBL.Wono,
  iif(dbo.TahvilDetailTBL.AmvalNo = 0, '', dbo.TahvilDetailTBL.AmvalNo) AS AmvalNo,
  dbo.TahvilDetailTBL.PartNumber,
  dbo.TahvilDetailTBL.PartNumberCount,
  dbo.TahvilDetailTBL.Serial,
  dbo.TahvilDetailTBL.Des,
  dbo.PartNumberTBL.PartName,
  dbo.PartNumberTBL.PartType,
  dbo.PartNumberTBL.PartModelName,
  CASE

    WHEN SUBSTRING(CAST(dbo.TahvilDetailTBL.PartNumber AS nvarchar), 1, 3) IN ('100') THEN 'fas fa-print fa-lg'
    WHEN SUBSTRING(CAST(dbo.TahvilDetailTBL.PartNumber AS nvarchar), 1, 3) IN ('105', '164') THEN 'fas fa-scanner-image fa-lg'
    WHEN SUBSTRING(CAST(dbo.TahvilDetailTBL.PartNumber AS nvarchar), 1, 3) IN ('102', '211') THEN 'fas fa-keyboard fa-lg'
    WHEN SUBSTRING(CAST(dbo.TahvilDetailTBL.PartNumber AS nvarchar), 1, 3) IN ('104', '213', '158', '167', '158') THEN 'fas fa-desktop fa-lg'
    WHEN SUBSTRING(CAST(dbo.TahvilDetailTBL.PartNumber AS nvarchar), 1, 3) IN ('103', '212') THEN 'fas fa-computer-speaker fa-lg'
    WHEN SUBSTRING(CAST(dbo.TahvilDetailTBL.PartNumber AS nvarchar), 1, 3) IN ('203') THEN 'fas fa-tablet-alt fa-lg'
    WHEN SUBSTRING(CAST(dbo.TahvilDetailTBL.PartNumber AS nvarchar), 1, 3) IN ('125', '215') THEN 'fas fa-mouse fa-lg'
    WHEN SUBSTRING(CAST(dbo.TahvilDetailTBL.PartNumber AS nvarchar), 1, 3) IN ('101') THEN 'fas fa-laptop fa-lg'
    WHEN SUBSTRING(CAST(dbo.TahvilDetailTBL.PartNumber AS nvarchar), 1, 3) IN ('118') THEN 'fas fa-server fa-lg'
    ELSE ''
  END AS Icon,
WorkOrder.ActiveWono

FROM dbo.TahvilHeaderTBL
INNER JOIN dbo.TahvilDetailTBL
  ON dbo.TahvilHeaderTBL.CodeTahvil = dbo.TahvilDetailTBL.CodeTahvil
INNER JOIN dbo.PartNumberTBL
  ON dbo.TahvilDetailTBL.PartNumber = dbo.PartNumberTBL.PartNumber
OUTER APPLY
(
	SELECT TOP 1 Wono AS ActiveWono FROM WorkOrderTBL WHERE dbo.WorkOrderTBL.Amval = CAST(dbo.TahvilDetailTBL.AmvalNo AS nvarchar) AND WorkReportTime IS NULL
) AS WorkOrder

GO


