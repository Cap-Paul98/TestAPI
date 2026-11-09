# TestAPI: una API ASP.NET Core en contenedor para llevar a AWS
 
API de demostración de la charla **Arquitectura Backend en la Nube** (AWS Student Community Day, Santa Cruz de la Sierra, 24 de octubre de 2026). Sirve para practicar el recorrido completo: imagen Docker → Amazon ECR → ECS con Fargate → balanceador interno → VPC Link → API Gateway.
 
La guía paso a paso con la configuración de cada servicio está en el artículo: **[enlace al artículo en AWS Builder Center]**.
 
> Es una API de práctica: no tiene autenticación y una de sus rutas devuelve datos aleatorios. No la expongas así en producción; la guía explica cómo agregar credenciales en API Gateway.
 
## Rutas
 
| Método | Ruta | Respuesta |
| --- | --- | --- |
| GET | `/api/Health` | `{"status":"Healthy"}` (ruta de salud para el balanceador) |
| GET | `/api/Home` | `{"message":"TestAPI is running"}` |
| GET | `/api/Home/hello-world` | un mensaje de saludo |
| GET | `/api/WeatherForecast` | cinco pronósticos con datos aleatorios |
 
Swagger (`/swagger`) solo se activa con `ASPNETCORE_ENVIRONMENT=Development`.
 
## Estructura
 
```
TestAPI/
├─ TestAPI/                 proyecto ASP.NET Core (net10.0)
│  ├─ Controllers/          Health, Home y WeatherForecast
│  ├─ Program.cs
│  └─ Dockerfile            build multietapa, expone el puerto 8080
├─ docker-compose.yml       para ejecutar en local con Visual Studio / Docker
└─ .dockerignore
```
 
## Construir la imagen
 
Desde la carpeta raíz del repositorio (donde está `TestAPI.slnx`), no desde dentro de `TestAPI/`:
 
```bash
docker build -f TestAPI/Dockerfile -t testapi:latest .
```
 
En un Mac con chip Apple, agrega `--platform linux/amd64` para que coincida con Fargate (x86).
 
## Ejecutar en local
 
```bash
docker run --rm -p 8080:8080 -e ASPNETCORE_HTTP_PORTS=8080 testapi:latest
```
 
Luego abre `http://localhost:8080/api/Health`.
 
`ASPNETCORE_HTTP_PORTS=8080` es la misma variable que se configura en la definición de tarea de ECS: le indica a .NET en qué puerto escuchar, sin HTTPS.
 
## Llevarla a AWS
 
Sigue el artículo. En resumen:
 
1. Crear un repositorio privado en ECR y subir la imagen.
2. Crear un clúster de ECS (solo Fargate), una definición de tarea (puerto 8080, `ASPNETCORE_HTTP_PORTS=8080`) y un servicio.
3. Crear un balanceador **interno** con la ruta de salud `/api/Health`.
4. Crear un VPC Link V2, una API HTTP con la ruta `ANY /{proxy+}` y una integración privada hacia el balanceador.
5. Al terminar, borrar todo: los balanceadores y las IPv4 públicas cobran por hora.
