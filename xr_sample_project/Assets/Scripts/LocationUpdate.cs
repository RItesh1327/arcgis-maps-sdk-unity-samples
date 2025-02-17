using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Geometry;
using System;
using System.Collections;
using UnityEngine;
using TMPro;
using Unity.XR.CoreUtils;

[Serializable]
public struct Coordinates
{
    public string name;
    public float longitude;
    public float latitude;
    public float altitude;

    public Coordinates(string name, float longitude, float latitude, float altitude)
    {
        this.name = name;
        this.longitude = longitude;
        this.latitude = latitude;
        this.altitude = altitude;
    }
}
public class LocationUpdate : MonoBehaviour
{
    private ArcGISMapComponent arcGISMapComponent;

    // TMP_InputField components for entering location data.
    [SerializeField] private TMP_InputField longitudeInputField;
    [SerializeField] private TMP_InputField latitudeInputField;
    [SerializeField] private TMP_InputField altitudeInputField;

    private GameObject menuManager;
    private GameObject XROrigin;

    private void Start()
    {
        XROrigin = FindObjectOfType<XROrigin>().gameObject;
        arcGISMapComponent = FindObjectOfType<ArcGISMapComponent>();
        menuManager = FindObjectOfType<VRMenuManager>().gameObject;
    }

    /// <summary>
    /// Called by the update button. Reads TMP input field values and transitions the player to the new location.
    /// </summary>
    public void UpdateLocation()
    {
        float longitude, latitude, altitude;

        // Try parsing the input values.
        if (float.TryParse(longitudeInputField.text, out longitude) &&
            float.TryParse(latitudeInputField.text, out latitude) &&
            float.TryParse(altitudeInputField.text, out altitude))
        {
            // Log the new coordinates.
            //Debug.LogError($"Location Updated: Longitude = {longitude}, Latitude = {latitude}, Altitude = {altitude}");

            // Start the transition coroutine using the custom coordinates.
            StartCoroutine(LoadIntoNewAreaWithFade(new Coordinates("CustomLocation", longitude, latitude, altitude)));
        }
        else
        {
            // Debug.LogError("Invalid input for location. Please enter valid numbers for longitude, latitude, and altitude.");
        }
    }

    private IEnumerator LoadIntoNewAreaWithFade(Coordinates location)
    {
        // Disable menu interactions and grab rays while teleporting.
        menuManager.GetComponent<VRMenuManager>().SetCurrentlyTeleporting(true);
        XROrigin.GetComponent<ActivateGrabRay>().currentlyTransporting = true;

        FadeScreen.Instance.FadeOut();

        // Wait for fade-out to complete.
        yield return new WaitForSeconds(FadeScreen.Instance.GetFadeDuration());

        SetPlayerSpawn(location.longitude, location.latitude, location.altitude);

        FadeScreen.Instance.FadeIn();

        // Re-enable menu interactions and grab rays.
        menuManager.GetComponent<VRMenuManager>().SetCurrentlyTeleporting(false);
        XROrigin.GetComponent<ActivateGrabRay>().currentlyTransporting = false;

        yield return new WaitForEndOfFrame();

        menuManager.GetComponent<VRMenuManager>().ToggleMenu(true);
    }

    /// <summary>
    /// Sets the ArcGIS Map's origin using the provided longitude, latitude, and altitude.
    /// </summary>
    private void SetNewArcGISMapOrigin(float longitude, float latitude, float altitude)
    {
        arcGISMapComponent = arcGISMapComponent ? arcGISMapComponent : FindObjectOfType<ArcGISMapComponent>();
        arcGISMapComponent.OriginPosition = new ArcGISPoint(longitude, latitude, altitude, ArcGISSpatialReference.WGS84());
    }

    /// <summary>
    /// Updates the player's spawn location and the map's origin.
    /// </summary>
    private void SetPlayerSpawn(float longitude, float latitude, float altitude)
    {
        // Update the map's origin with the provided altitude.
        SetNewArcGISMapOrigin(longitude, latitude, altitude);

        XROrigin = XROrigin ? XROrigin : FindObjectOfType<XROrigin>().gameObject;

        ArcGISLocationComponent playerLocation = XROrigin.GetComponent<ArcGISLocationComponent>();
        if (playerLocation)
        {
            playerLocation.Position = new ArcGISPoint(longitude, latitude, altitude, ArcGISSpatialReference.WGS84());
        }
    }
}
