
--________________________________ INSERTAR ROLES ________________________________
INSERT into rol(descripcion,esActivo) VALUES
('Administrador',1),
('Empleado',1),
('Supervisor',1)


--________________________________ INSERTAR USUARIO ________________________________
SELECT * FROM Usuario
--clave : 123
INSERT INTO Usuario(nombre,correo,telefono,idRol,urlFoto,nombreFoto,clave,esActivo) VALUES
('codigo estudiante','codigo@example.com','909090',1,'','','a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3',1)

--________________________________ RECURSOS DE FIREBASE_STORAGE Y CORREO ________________________________
--(AQUI DEBES INCLUIR TUS PROPIAS CLAVES Y CRENDENCIALES)

INSERT INTO Configuracion(recurso,propiedad,valor) VALUES
('FireBase_Storage','email',''),
('FireBase_Storage','clave',''),
('FireBase_Storage','ruta',''),
('FireBase_Storage','api_key',''),
('FireBase_Storage','carpeta_usuario','IMAGENES_USUARIO'),
('FireBase_Storage','carpeta_producto','IMAGENES_PRODUCTO'),
('FireBase_Storage','carpeta_logo','IMAGENES_LOGO')

INSERT INTO Configuracion(recurso,propiedad,valor) VALUES
('Servicio_Correo','correo',''),
('Servicio_Correo','clave',''),
('Servicio_Correo','alias','MiTienda.com'),
('Servicio_Correo','host','smtp.gmail.com'),
('Servicio_Correo','puerto','587')


--________________________________ INSERTAR NEGOCIO ________________________________
select * FROM Negocio

INSERT INTO Negocio(idNegocio,urlLogo,nombreLogo,numeroDocumento,nombre,correo,direccion,telefono,porcentajeImpuesto,simboloMoneda)
VALUES(1,'','','','','','','',0,'')


--________________________________ INSERTAR CATEGORIAS ________________________________
SELECT * FROM Categoria

INSERT INTO Categoria(descripcion,esActivo) VALUES
('Computadoras',1),
('Laptops',1),
('Teclados',1),
('Monitores',1),
('Microfonos',1)


--________________________________ INSERTAR TIPO DOCUMENTO VENTA ________________________________

select * FROM TipoDocumentoVenta

INSERT INTO TipoDocumentoVenta(descripcion,esActivo) VALUES
('Boleta',1),
('Factura',1)


--________________________________ INSERTAR NUMERO CORRELATIVO ________________________________
select * FROM NumeroCorrelativo
--000001
INSERT INTO NumeroCorrelativo(ultimoNumero,cantidadDigitos,gestion,fechaActualizacion) VALUES
(0,6,'venta',getdate())


--________________________________ INSERTAR MENUS ________________________________
select * FROM Menu

--*menu padre
INSERT INTO Menu(descripcion,icono,controlador,paginaAccion,esActivo) VALUES
('DashBoard','fas fa-fw fa-tachometer-alt','DashBoard','Index',1)

INSERT INTO Menu(descripcion,icono,esActivo) VALUES
('Administración','fas fa-fw fa-cog',1),
('Inventario','fas fa-fw fa-clipboard-list',1),
('Ventas','fas fa-fw fa-tags',1),
('Reportes','fas fa-fw fa-chart-area',1)


--*menu hijos Administracion
INSERT INTO Menu(descripcion,idMenuPadre, controlador,paginaAccion,esActivo) VALUES
('Usuarios',2,'Usuario','Index',1),
('Negocio',2,'Negocio','Index',1)


--*menu hijos - Inventario
INSERT INTO Menu(descripcion,idMenuPadre, controlador,paginaAccion,esActivo) VALUES
('Categorias',3,'Categoria','Index',1),
('Productos',3,'Producto','Index',1)

--*menu hijos - Ventas
INSERT INTO Menu(descripcion,idMenuPadre, controlador,paginaAccion,esActivo) VALUES
('Nueva Venta',4,'Venta','NuevaVenta',1),
('Historial Venta',4,'Venta','HistorialVenta',1)

--*menu hijos - Reportes
INSERT INTO Menu(descripcion,idMenuPadre, controlador,paginaAccion,esActivo) VALUES
('Reporte de Ventas',5,'Reporte','Index',1)


UPDATE Menu SET idMenuPadre = idMenu WHERE idMenuPadre is null


--________________________________ INSERTAR ROL MENU ________________________________
select * FROM Menu
select * FROM RolMenu
SELECT * FROM ROL

--*administrador
INSERT INTO RolMenu(idRol,idMenu,esActivo) VALUES
(1,1,1),
(1,6,1),
(1,7,1),
(1,8,1),
(1,9,1),
(1,10,1),
(1,11,1),
(1,12,1)

--*Empleado
INSERT INTO RolMenu(idRol,idMenu,esActivo) VALUES
(2,10,1),
(2,11,1)

--*Supervisor
INSERT INTO RolMenu(idRol,idMenu,esActivo) VALUES
(3,8,1),
(3,9,1),
(3,10,1),
(3,11,1)