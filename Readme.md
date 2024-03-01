# Tranquilidad 360

Este repositorio aloja al componente de visualizacion de photos, videos y escenas 3D del sistema Tranquilidad 360. Muchas veces se hace referencia a este componente como "Servidor", ya que es quien va a recibir los mensajes por parte del cliente mobile.

Link al lado del cliente, escrito en Flutter: [Flutter](https://github.com/Simonlanzavecchia/TranquiFlut)

### Información de utilidad:

1. Se pueden encontrar builds tanto del cliente como del servidor en la siguiente [carpeta de drive](https://drive.google.com/drive/folders/1pFFIv_WBP2JG6kbnuE-MjrYfbFHVgd9C?usp=sharing)

2. Unity Editor:
	  - Versión: [2021.3.32f1](https://unity.com/releases/editor/archive#download-archive-2021) con los módulos "OpenJDK" y "Android SDK & NDK Support"

	- La ubicacion de la escena inicial es: "\Assets\Niveles\Scenes\Main.unity"

3. Las imagenes y videos de ejemplo no estan alojados en Github debido a su peso (y posibles derechos de autor), pero se encuentran subidos a una [carpeta en drive](https://drive.google.com/drive/folders/15eV8e_d9GiqZK-1TQkJpykFT8Hkizrxa?usp=sharing).
    - Los archivos de video se deben descargar y ubicar en "\Assets\Visuales\Content\Videos"
    - Los archivos de imagenes se deben descargar y ubicar en "\Assets\Visuales\Content\Photos"
    - nota: Unity localiza los archivos por medio de otro archivo con extension .meta (de mismo nombre), por lo que la ubicación de las fotos/videos no debería provocar problemas. Las ubicaciones son sugeridas para mantener la organizacion del proyecto.

4. **Network discovery solo funciona en Android.** Esto es porque se realiza mediante llamados a un archivo .jar, cosa que se realiza mediante la JNI (Java Native Interface) solo presente en Android.
	  - Se podría utilizar alguna biblioteca para que funcione en el editor, pero es trabajo extra que no consideramos que valga la pena.
	  - La ip y puerto se imprimen por consola para cuando se ejecuta desde el editor.
