using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGroupViewController : MonoBehaviour
{
    [SerializeField]
    private GameObject _roomListing;
    private List<RoomListingController> _roomListingButtons = new List<RoomListingController>();
    private List<RoomListingController> RoomListingButtons
    {
        get { return _roomListingButtons; }
    }

    private void OnReceivedRoomListUpdate()
    {
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();

        foreach (RoomInfo room in rooms)
        {
            RoomReceived(room);
        }

        RemoveOldRooms();
    }

    private void RoomReceived(RoomInfo room)
    {
        int index = RoomListingButtons.FindIndex(x => x.RoomName == room.Name);

        if (index == -1)
        {
            if (room.IsVisible && room.PlayerCount < room.MaxPlayers)
            {
                GameObject roomListingObj = Instantiate(_roomListing);
                roomListingObj.transform.SetParent(transform, false);

                RoomListingController roomListing = roomListingObj.GetComponent<RoomListingController>();
                RoomListingButtons.Add(roomListing);

                index = (RoomListingButtons.Count - 1);
            }
        }

        if (index != -1)
        {
            RoomListingController roomListing = RoomListingButtons[index];
            roomListing.SetRoomNameText(room.Name);
            roomListing.Updated = true;
        }
    }

    private void RemoveOldRooms()
    {
        List<RoomListingController> removeRooms = new List<RoomListingController>();
        foreach (RoomListingController roomListing in RoomListingButtons)
        {
            if (!roomListing.Updated)
                removeRooms.Add(roomListing);
            else
                roomListing.Updated = false;
        }
        foreach (RoomListingController roomListing in removeRooms)
        {
            GameObject roomListingObj = roomListing.gameObject;
            RoomListingButtons.Remove(roomListing);
            Destroy(roomListingObj);
        }
    }
}
