using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicationProtocol.Dto
{
    [Serializable]
    public class TransformDto
    {
        public float posX;
        public float posZ;
        public float rotaY;

        public TransformDto()
        {

        }
        public TransformDto(float posx,float posz,float rotay)
        {
            posX = posx;
            posZ= posz;
            rotaY = rotay;
        }
    }
}
