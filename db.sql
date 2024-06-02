USE [TFC]
GO
/****** Object:  Table [dbo].[Inventario]    Script Date: 02/06/2024 22:10:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Inventario](
	[SKU] [varchar](30) NOT NULL,
	[Talla] [decimal](10, 1) NULL,
	[Precio] [decimal](10, 2) NULL,
	[Cantidad] [int] NULL,
	[Cliente] [varchar](25) NULL,
	[Clave] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK__Inventar__CA1ECF0CA8A4A0D6] PRIMARY KEY CLUSTERED 
(
	[Clave] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Usuarios]    Script Date: 02/06/2024 22:10:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Usuarios](
	[Usuario] [varchar](25) NOT NULL,
	[Nombre] [varchar](100) NULL,
	[Email] [varchar](50) NULL,
	[Contrasenia] [varchar](max) NULL,
	[Administrador] [tinyint] NULL,
	[FechaRegistro] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[Usuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Zapatillas]    Script Date: 02/06/2024 22:10:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Zapatillas](
	[SKU] [varchar](30) NOT NULL,
	[Marca] [varchar](50) NULL,
	[Modelo] [varchar](50) NULL,
	[Imagen] [varchar](max) NULL,
	[FechaLanzamiento] [date] NOT NULL,
	[Nombre] [varchar](50) NULL,
	[Descripcion] [varchar](100) NULL,
	[Link] [nvarchar](max) NULL,
 CONSTRAINT [PK__Zapatill__CA1ECF0CB0250578] PRIMARY KEY CLUSTERED 
(
	[SKU] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Inventario] ON 

INSERT [dbo].[Inventario] ([SKU], [Talla], [Precio], [Cantidad], [Cliente], [Clave]) VALUES (N'FV5029-141', CAST(36.0 AS Decimal(10, 1)), CAST(300.00 AS Decimal(10, 2)), 0, N'test', 47)
INSERT [dbo].[Inventario] ([SKU], [Talla], [Precio], [Cantidad], [Cliente], [Clave]) VALUES (N'FV5029-141', CAST(36.5 AS Decimal(10, 1)), CAST(300.00 AS Decimal(10, 2)), 0, N'test', 48)
INSERT [dbo].[Inventario] ([SKU], [Talla], [Precio], [Cantidad], [Cliente], [Clave]) VALUES (N'FV5029-141', CAST(40.0 AS Decimal(10, 1)), CAST(300.00 AS Decimal(10, 2)), 0, N'test', 49)
INSERT [dbo].[Inventario] ([SKU], [Talla], [Precio], [Cantidad], [Cliente], [Clave]) VALUES (N'FV5029-141', CAST(47.0 AS Decimal(10, 1)), CAST(300.00 AS Decimal(10, 2)), 0, N'test', 50)
INSERT [dbo].[Inventario] ([SKU], [Talla], [Precio], [Cantidad], [Cliente], [Clave]) VALUES (N'FV5029-141', CAST(45.0 AS Decimal(10, 1)), CAST(300.00 AS Decimal(10, 2)), 8, N'test', 51)
INSERT [dbo].[Inventario] ([SKU], [Talla], [Precio], [Cantidad], [Cliente], [Clave]) VALUES (N'DZ5485-612', CAST(36.0 AS Decimal(10, 1)), CAST(400.00 AS Decimal(10, 2)), 2, N'test', 56)
INSERT [dbo].[Inventario] ([SKU], [Talla], [Precio], [Cantidad], [Cliente], [Clave]) VALUES (N'DZ5485-612', CAST(40.0 AS Decimal(10, 1)), CAST(400.00 AS Decimal(10, 2)), 1, N'test', 57)
INSERT [dbo].[Inventario] ([SKU], [Talla], [Precio], [Cantidad], [Cliente], [Clave]) VALUES (N'DZ5485-612', CAST(45.0 AS Decimal(10, 1)), CAST(400.00 AS Decimal(10, 2)), 4, N'test', 58)
INSERT [dbo].[Inventario] ([SKU], [Talla], [Precio], [Cantidad], [Cliente], [Clave]) VALUES (N'CV1363-700', NULL, CAST(1.00 AS Decimal(10, 2)), 0, N'test', 72)
INSERT [dbo].[Inventario] ([SKU], [Talla], [Precio], [Cantidad], [Cliente], [Clave]) VALUES (N'CV1363-700', CAST(36.5 AS Decimal(10, 1)), CAST(1.00 AS Decimal(10, 2)), 5, N'test', 80)
INSERT [dbo].[Inventario] ([SKU], [Talla], [Precio], [Cantidad], [Cliente], [Clave]) VALUES (N'CV1363-700', CAST(36.0 AS Decimal(10, 1)), CAST(1.00 AS Decimal(10, 2)), 1, N'test', 82)
SET IDENTITY_INSERT [dbo].[Inventario] OFF
GO
INSERT [dbo].[Usuarios] ([Usuario], [Nombre], [Email], [Contrasenia], [Administrador], [FechaRegistro]) VALUES (N'astasfg', N'test25125', N'asga@g.wagh', N'54d5cb2d332dbdb4850293caae4559ce88b65163f1ea5d4e4b3ac49d772ded14', NULL, CAST(N'2024-05-29' AS Date))
INSERT [dbo].[Usuarios] ([Usuario], [Nombre], [Email], [Contrasenia], [Administrador], [FechaRegistro]) VALUES (N'test', N'test', N'test@g.c', N'b460b1982188f11d175f60ed670027e1afdd16558919fe47023ecd38329e0b7f', NULL, CAST(N'2024-05-28' AS Date))
INSERT [dbo].[Usuarios] ([Usuario], [Nombre], [Email], [Contrasenia], [Administrador], [FechaRegistro]) VALUES (N'test2', N'test2', N'test@ga.c', N'54d5cb2d332dbdb4850293caae4559ce88b65163f1ea5d4e4b3ac49d772ded14', 1, CAST(N'2024-05-22' AS Date))
INSERT [dbo].[Usuarios] ([Usuario], [Nombre], [Email], [Contrasenia], [Administrador], [FechaRegistro]) VALUES (N'test3', N'testsetsts', N'test@g.asc', N'b460b1982188f11d175f60ed670027e1afdd16558919fe47023ecd38329e0b7f', NULL, CAST(N'2024-04-21' AS Date))
GO
INSERT [dbo].[Zapatillas] ([SKU], [Marca], [Modelo], [Imagen], [FechaLanzamiento], [Nombre], [Descripcion], [Link]) VALUES (N'555088-711', N'Jordan', N'Air Jordan 1', N'https://images.stockx.com/images/Air-Jordan-1-Retro-High-OG-Yellow-Toe-Product.jpg', CAST(N'2024-05-24' AS Date), N'Jordan 1 Retro High OG Taxi', NULL, N'https://stockx.pvxt.net/c/4722526/1023711/9060?prodsku=5424c96b-28a1-49ab-bb95-5967c46b967b&u=https%3A%2F%2Fstockx.com%2Fair-jordan-1-retro-high-og-yellow-toe%3Fcountry%3DUS%26currencyCode%3DUSD%26size%3D11&intsrc=CATF_7942')
INSERT [dbo].[Zapatillas] ([SKU], [Marca], [Modelo], [Imagen], [FechaLanzamiento], [Nombre], [Descripcion], [Link]) VALUES (N'CV1363-700', N'Nike', N'Nike Running Other', N'https://images.stockx.com/images/Nike-Vaporwaffle-sacai-Tour-Yellow-Stadium-Green-Product.jpg', CAST(N'2024-05-27' AS Date), N'Nike Vaporwaffle sacai Tour Yellow Stadium Green', NULL, N'https://stockx.pvxt.net/c/4722526/1023711/9060?prodsku=2765da16-0c85-4b40-b3f0-b5ac8f8bd846&u=https%3A%2F%2Fstockx.com%2Fnike-vaporwaffle-sacai-tour-yellow-stadium-green%3Fcountry%3DUS%26currencyCode%3DUSD%26size%3D9&intsrc=CATF_7942')
INSERT [dbo].[Zapatillas] ([SKU], [Marca], [Modelo], [Imagen], [FechaLanzamiento], [Nombre], [Descripcion], [Link]) VALUES (N'CZ0790-801', N'Jordan', N'Air Jordan 1', N'https://images.stockx.com/images/Air-Air-Jordan-1-Low-OG-Shattered-Backboard-Product.jpg', CAST(N'2024-05-21' AS Date), N'Jordan 1 Low Starfish', NULL, N'https://stockx.pvxt.net/c/4722526/1023711/9060?prodsku=21e0a9f7-8d2e-4ae3-b8c9-461561be60ec&u=https%3A%2F%2Fstockx.com%2Fair-jordan-1-low-og-shattered-backboard%3Fcountry%3DUS%26currencyCode%3DUSD%26size%3D12&intsrc=CATF_7942')
INSERT [dbo].[Zapatillas] ([SKU], [Marca], [Modelo], [Imagen], [FechaLanzamiento], [Nombre], [Descripcion], [Link]) VALUES (N'DD1391-103', N'Nike', N'Nike Dunk', N'https://images.stockx.com/images/Nike-Dunk-Low-Grey-Fog-Product.jpg', CAST(N'2024-06-03' AS Date), N'Nike Dunk Low Grey Fog', NULL, N'https://stockx.pvxt.net/c/4722526/1023711/9060?prodsku=b5b46e42-035c-4616-9537-6f8f48b63e8d&u=https%3A%2F%2Fstockx.com%2Fnike-dunk-low-grey-fog%3Fcountry%3DUS%26currencyCode%3DUSD%26size%3D15&intsrc=CATF_7942')
INSERT [dbo].[Zapatillas] ([SKU], [Marca], [Modelo], [Imagen], [FechaLanzamiento], [Nombre], [Descripcion], [Link]) VALUES (N'DZ5485-612', N'Jordan', N'Air Jordan 1', N'https://images.stockx.com/images/Air-Jordan-1-Retro-High-OG-Chicago-Reimagined-Product.jpg', CAST(N'2024-05-31' AS Date), N'Jordan 1 Retro High OG Chicago Lost and Found', N'Prueba', N'https://stockx.pvxt.net/c/4722526/1023711/9060?prodsku=c63bef6c-d1cb-4b26-967e-cb2c5bf31e28&u=https%3A%2F%2Fstockx.com%2Fair-jordan-1-retro-high-og-chicago-reimagined-lost-and-found%3Fcountry%3DUS%26currencyCode%3DUSD%26size%3D10&intsrc=CATF_7942')
INSERT [dbo].[Zapatillas] ([SKU], [Marca], [Modelo], [Imagen], [FechaLanzamiento], [Nombre], [Descripcion], [Link]) VALUES (N'FV5029-141', N'Jordan', N'Air Jordan 4', N'https://images.stockx.com/images/Air-Jordan-4-Retro-Military-Blue-2024-Product.jpg', CAST(N'2024-05-05' AS Date), N'Jordan 4 Retro Military Blue (2024)', NULL, N'https://stockx.pvxt.net/c/4722526/1023711/9060?prodsku=58a49ac8-d099-4c50-bac7-4d5501388f01&u=https%3A%2F%2Fstockx.com%2Fair-jordan-4-retro-military-blue-2024%3Fcountry%3DUS%26currencyCode%3DUSD%26size%3D11&intsrc=CATF_7942')
INSERT [dbo].[Zapatillas] ([SKU], [Marca], [Modelo], [Imagen], [FechaLanzamiento], [Nombre], [Descripcion], [Link]) VALUES (N'FZ3129-200', N'Nike', N'Nike SB SB Dunk Low', N'https://images.stockx.com/images/Nike-SB-Dunk-Low-Big-Money-Savings-Product.jpg', CAST(N'2024-05-07' AS Date), N'Nike SB Dunk Low Big Money Savings', NULL, N'https://stockx.pvxt.net/c/4722526/1023711/9060?prodsku=b0947f9f-0976-43f2-9c9e-33a8cf8bee63&u=https%3A%2F%2Fstockx.com%2Fnike-sb-dunk-low-big-money-savings%3Fcountry%3DUS%26currencyCode%3DUSD%26size%3D11&intsrc=CATF_7942')
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Usuarios__A9D105346FE6AFD6]    Script Date: 02/06/2024 22:10:10 ******/
ALTER TABLE [dbo].[Usuarios] ADD UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Inventario]  WITH CHECK ADD  CONSTRAINT [FK_Inventario_Usuarios] FOREIGN KEY([Cliente])
REFERENCES [dbo].[Usuarios] ([Usuario])
GO
ALTER TABLE [dbo].[Inventario] CHECK CONSTRAINT [FK_Inventario_Usuarios]
GO
ALTER TABLE [dbo].[Inventario]  WITH CHECK ADD  CONSTRAINT [FK_Inventario_Zapatillas] FOREIGN KEY([SKU])
REFERENCES [dbo].[Zapatillas] ([SKU])
GO
ALTER TABLE [dbo].[Inventario] CHECK CONSTRAINT [FK_Inventario_Zapatillas]
GO
