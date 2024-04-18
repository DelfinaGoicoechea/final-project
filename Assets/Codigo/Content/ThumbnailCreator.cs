using System.IO;
using UnityEngine;
 
public class ThumbnailCreator
{
    private static Camera _camera; 
    public static Texture2D Capture(string name)
    {
        if (_camera == null)
            _camera = GameObject.FindWithTag("ThumbnailCamera").GetComponent<Camera>();
        RenderTexture activeRenderTexture = RenderTexture.active;
        RenderTexture.active = _camera.targetTexture;

        _camera.Render();
        Debug.Log(_camera.targetTexture);
 
        Texture2D image = new Texture2D(_camera.targetTexture.width, _camera.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, _camera.targetTexture.width, _camera.targetTexture.height), 0, 0);
        image.Apply();
        RenderTexture.active = activeRenderTexture;
 
        byte[] bytes = image.EncodeToPNG();
        char OsSep = Path.DirectorySeparatorChar;
        if (!Directory.Exists(Application.persistentDataPath + $"{OsSep}thumbnails")){
            Directory.CreateDirectory(Application.persistentDataPath + $"{OsSep}thumbnails");
        }
        File.WriteAllBytes(Application.persistentDataPath + $"{OsSep}thumbnails{OsSep}" + name + ".png", bytes);
        return image;
    }
}
