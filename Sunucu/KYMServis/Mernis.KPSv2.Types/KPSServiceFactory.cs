using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.ServiceModel.Channels;
using System.IdentityModel.Tokens;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Microsoft.IdentityModel.SecurityTokenService;

namespace Mernis.KPSv2.Types
{
    public class KPSServiceFactory
    {
        #region Constructors

        private KPSServiceFactory() { }

        #endregion

        #region Fields

        private static KPSServiceFactory instance;

        private SecurityToken token;

        #endregion

        #region Properties

        public static KPSServiceFactory Instance
        {
            get
            {
                if (instance == null)
                    instance = new KPSServiceFactory();

                return instance;
            }
        }

        #endregion

        #region Methods

        public SecurityToken CreateToken()
        {
            if (token == null || token.ValidTo <= DateTime.Now.ToUniversalTime())
            {
                Microsoft.IdentityModel.Protocols.WSTrust.WSTrustChannelFactory trustChannelFactory = new Microsoft.IdentityModel.Protocols.WSTrust.WSTrustChannelFactory("STSIssuerService");
                trustChannelFactory.TrustVersion = TrustVersion.WSTrust13;
                trustChannelFactory.Credentials.UserName.UserName = KPSConfiguration.Instance.Username;
                trustChannelFactory.Credentials.UserName.Password = KPSConfiguration.Instance.Password;

                Microsoft.IdentityModel.Protocols.WSTrust.WSTrustChannel channel = (Microsoft.IdentityModel.Protocols.WSTrust.WSTrustChannel)trustChannelFactory.CreateChannel();
                RequestSecurityToken rst = new RequestSecurityToken(RequestTypes.Issue);
                rst.AppliesTo = new EndpointAddress(KPSConfiguration.Instance.EndPoint);
                RequestSecurityTokenResponse rstr;

                token = channel.Issue(rst, out rstr);
            }

            return token;
        }

        #endregion
    }
}
