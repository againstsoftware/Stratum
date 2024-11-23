using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class GramophoneAnimations : MonoBehaviour
{
    [SerializeField] private GameObject _Vinyl, _Handle, _Volume;

    // Handle
    Quaternion _targetRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
    private float _rotationSpeed = 90f;
    private bool _isHandleMoving = false;

    // Volume
    private bool _isVolumeMoving = false;
    Quaternion _targetRotationVol = Quaternion.Euler(new Vector3(0f, 0f, 0f));

    // Vinyl
    [SerializeField] private GameObject _Needle;
    private Vector3 _targetPosVinyl;
    private Quaternion _targetRotationVinyl;
    private bool _isVinylMoving = false; 
    private Vector3 _initPosVinyl; 
    private Quaternion _initPosNeedle;
    private int _animVinylState = 0;

    void Start()
    {
        CheckHandleInitState();

    }

    void Update()
    {
        if (_isHandleMoving)
        {
            _Handle.transform.localRotation = Quaternion.RotateTowards(_Handle.transform.localRotation, _targetRotation, _rotationSpeed * Time.deltaTime);

            if (Quaternion.Angle(_Handle.transform.localRotation, _targetRotation) < 0.01f)
            {
                _isHandleMoving = false;
            }
        }

        if (_isVolumeMoving)
        {
            _Volume.transform.localRotation = Quaternion.RotateTowards(_Volume.transform.localRotation, _targetRotationVol, _rotationSpeed * Time.deltaTime);

            if (Quaternion.Angle(_Volume.transform.localRotation, _targetRotationVol) < 0.01f)
            {
                _isHandleMoving = false;
            }
        }

        if (_isVinylMoving)
        {
            switch(_animVinylState)
            {
                case 0:
                    _Vinyl.GetComponent<Collider>().enabled = false;
                    _Needle.transform.localRotation = Quaternion.RotateTowards(_Needle.transform.localRotation, Quaternion.Euler(new Vector3(60f, -90f, -90f)), _rotationSpeed * Time.deltaTime);
                    _Vinyl.transform.position = Vector3.MoveTowards(_Vinyl.transform.position, _targetPosVinyl, 1f * Time.deltaTime);

                    if((_targetPosVinyl.y - _Vinyl.transform.position.y) < 0.1f) _animVinylState = 1;
                    break;
                case 1:
                    _Vinyl.transform.rotation = Quaternion.RotateTowards(_Vinyl.transform.rotation, _targetRotationVinyl, _rotationSpeed * Time.deltaTime);

                    if (Quaternion.Angle(_Vinyl.transform.rotation, _targetRotationVinyl) < 0.01f) _animVinylState = 2;
                    break;
                case 2:
                    _Vinyl.transform.position = Vector3.MoveTowards(_Vinyl.transform.position, _initPosVinyl, 1f * Time.deltaTime);
                    _Needle.transform.localRotation = Quaternion.RotateTowards(_Needle.transform.localRotation, _initPosNeedle, _rotationSpeed * Time.deltaTime);

                    if((_Vinyl.transform.position.y - _initPosVinyl.y) < 0.01f) 
                    {
                        _isVinylMoving = false;
                        _animVinylState = 0;
                        _Vinyl.GetComponent<Collider>().enabled = true;
                    }
                    break;
            }
        }


    }

    private void CheckHandleInitState()
    {
        int qualityLevel = PlayerPrefs.GetInt(GamePrefs.QualityPrefKey, 2);
        switch (qualityLevel)
        {
            case 0:
                _Handle.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
                break;
            case 1:
                _Handle.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -91f));
                break;
            case 2:
                _Handle.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -181f));
                break;
        }
    }

    public void HandleAnim()
    {
        int qualityLevel = PlayerPrefs.GetInt("graphics_quality", 2);

        switch (qualityLevel)
        {
            case 0:
                _targetRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
                _isHandleMoving = true;
                break;
            case 1:
                _targetRotation = Quaternion.Euler(new Vector3(0f, 0f, -91f));
                _isHandleMoving = true;
                break;
            case 2:
                _targetRotation = Quaternion.Euler(new Vector3(0f, 0f, -180f));
                _isHandleMoving = true;
                break;
        }
    }

    public void VolumeAnim()
    {
        _targetRotationVol = _Volume.transform.localRotation * Quaternion.Euler(new Vector3(-90f, 0f, 0f));
        _isVolumeMoving = true;
    }

    public void VinylAnim()
    {
        // rotar la needle
        // elevar el disco 
        //      rotar el disco
        // bajarlo 
        _initPosNeedle = _Needle.transform.localRotation;
        _initPosVinyl = _Vinyl.transform.position;
        _targetPosVinyl = _initPosVinyl + new Vector3(0f, 0.3f, 0f);
        _targetRotationVinyl = _Vinyl.transform.localRotation * Quaternion.Euler(new Vector3(0f, 0f, -180f));

        _isVinylMoving = true;
    }


}
