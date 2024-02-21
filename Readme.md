# Tranquilidad 360

Link al repo de Flutter: [Flutter](https://github.com/Simonlanzavecchia/TranquiFlut)

- Unity Editor:
  - Versión: [2021.3.32f1](https://unity.com/releases/editor/archive#download-archive-2021) con los módulos "OpenJDK" y "Android SDK & NDK Support"

- La ubicacion de la escena inicial es: "\Assets\Niveles\Scenes\Main.unity"

- Los videos no estan alojados en Github debido a su peso (y derechos de autor), pero se encuentran subidos a una [carpeta en drive](https://drive.google.com/drive/folders/15eV8e_d9GiqZK-1TQkJpykFT8Hkizrxa?usp=sharing).
  - Los archivos de video se deben descargar y ubicar (al menos de momento) en "\Assets\Visuales\Content\Videos"

- **Network discovery solo funciona en Android.** Esto es porque se realiza mediante llamados a un archivo .jar, cosa que se realiza mediante la JNI (Java Native Interface) solo presente en Android.
  - Se podría utilizar alguna biblioteca para que funcione en el editor, pero es trabajo extra que no consideramos que valga la pena.
  - La ip y puerto se imprimen por consola para cuando se ejecuta desde el editor.
