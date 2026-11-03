using AdaptiveOpponent.Game;
using NUnit.Framework;

namespace AdaptiveOpponent.Tests
{
    public class CommitmentTests
    {
        [Test]
        public void HashMatchesAnIndependentSha256()
        {
            // python: hashlib.sha256(b"3|000102030405060708090a0b0c0d0e0f").hexdigest()
            Assert.AreEqual("0e81508e031b584dd4ca2045ac10c5d9939bd64a8a4318f0496e3346b3cd4e72",
                Commitment.HashOf(3, "000102030405060708090a0b0c0d0e0f"));
        }

        [Test]
        public void ACommitmentOpensWithItsNonceAndNoOtherResponse()
        {
            Commitment c = Commitment.For(4);
            Assert.AreEqual(32, c.Nonce.Length);
            Assert.AreEqual(c.Hash, Commitment.HashOf(4, c.Nonce));
            for (int r = 0; r < 6; r++)
            {
                if (r != 4)
                {
                    Assert.AreNotEqual(c.Hash, Commitment.HashOf(r, c.Nonce));
                }
            }
            Assert.AreNotEqual(c.Nonce, Commitment.For(4).Nonce, "every commitment has a fresh nonce");
        }

        [Test]
        public void SessionSeedsFitAJsonNumberExactly()
        {
            for (int i = 0; i < 100; i++)
            {
                Assert.Less(Commitment.NewSessionSeed(), 1UL << 53);
            }
        }
    }
}
