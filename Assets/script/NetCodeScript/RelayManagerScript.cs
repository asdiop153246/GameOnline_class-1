using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using QFSW.QC;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Netcode;
using System.Threading.Tasks;
using TMPro;
//using ParrelSync;

public class RelayManagerScript : Singleton<RelayManagerScript>
{
    private string joinCode;
    public string JoinCode
    {
        get { return joinCode; }
    }
    public UnityTransport Transport => 
        NetworkManager.Singleton.GetComponent<UnityTransport>();
    public bool IsRelayEnabled => 
        Transport != null && Transport.Protocol == UnityTransport.ProtocolType.RelayUnityTransport;
    private async void Start()
    {
        InitializationOptions options = new InitializationOptions();
#if UNITY_EDITOR //ทำให้ Unity clones เป็นคนละเครื่องกัน
        //options.SetProfile(ClonesManager.IsClone() ? ClonesManager.GetArgument() : "Primary");
#endif
        await UnityServices.InitializeAsync(options);
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Signed in = " + AuthenticationService.Instance.PlayerId);
            };
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    [Command]
    public async Task CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            Debug.Log("Join code = " + joinCode);
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    public async Task JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("Joining Relay with " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
}
