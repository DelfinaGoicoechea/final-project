using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Esta clase existe porque en la metrica "MetricaSoltarFrutas" necesito acceder a los 
/// Direct/Ray Interactors, pero los GameObjects estan inactivos y no puedo encontrarlos
/// con "FindGameObjectsWithTag". Por lo que guardo las referencias relevantes desde el editor.
/// 
/// Si vuelve a presentarse un problema similar en el Controlador, seria conveniente reutilizar 
/// esta clase para centralizar las referencias al mismo.
/// </summary>
public class ControllerReferences : MonoBehaviour
{
    public List<XRDirectInteractor> directInteractors;
    public List<XRRayInteractor> rayInteractors;
}
