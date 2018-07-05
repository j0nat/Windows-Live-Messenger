using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLMData.Enums
{
    public enum PacketName
    {
        requestContactList,
        requestNewContact,
        requestLogin,
        sendLoginResult,
        sendNudge,
        sendUserUpdate,
        sendMessage,
        sendContact,
        sendUserContact,
        sendFriendRequestToUser,
        sendFriendRequestResponse,
        sendContactDelete,
        sendContactBlock,
        sendWritingStatus,
        sendGameRequest,
        sendPersonalUserUpdate
    };
}
