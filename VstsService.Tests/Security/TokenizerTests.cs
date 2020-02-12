using Microsoft.IdentityModel.Tokens;
using SecurePipelineScan.VstsService.Security;
using Shouldly;
using System;
using System.Security.Claims;
using Xunit;

namespace Functions.Tests
{
    public class TokenizerTests
    {
        [Fact]
        public void IncludeCustomDataInClaims()
        {
            var tokenizer = new Tokenizer(Guid.NewGuid().ToString());
            var token = tokenizer.Token(new Claim("project", "TAS"));

            var principal = tokenizer.Principal(token);
            principal.ShouldNotBeNull();
            principal.HasClaim("project", "TAS").ShouldBeTrue();
        }

        [Fact]
        public void IncludeServerSecretInToken()
        {
            var tokenizer1 = new Tokenizer(Guid.NewGuid().ToString());
            var tokenizer2 = new Tokenizer(Guid.NewGuid().ToString());
            var token = tokenizer1.Token();

            tokenizer1.Principal(token).ShouldNotBeNull();
            Assert.Throws<SecurityTokenInvalidSignatureException>(() => tokenizer2.Principal(token));
        }

        [Fact]
        public void ThrowOnNullSecret()
        {
            Assert.Throws<ArgumentNullException>(() => new Tokenizer(null));
        }
    }
}