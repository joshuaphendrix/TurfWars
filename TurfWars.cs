using System;
using System.Collections.Generic;

public class TurfWarsGame
{
    private Dictionary<Geo.Box, Claim> _claims;
    private Dictionary<String, User> _users;

    public TurfWarsGame()
    {
        _users = new Dictionary<string, User>();
        _claims = new Dictionary<Geo.Box, Claim>();
    }

    public Geo.Box addClaim(string userName, double lat, double lng)
    {
        Claim tile = new Claim(lat, lng);

        if (!_users.ContainsKey(userName))
        {
            User user = new User(userName);
            _users.Add(userName, user);
        }

        if (!_claims.ContainsKey(tile.box))
        {
            _claims.Add(tile.box, tile);
        }

        double changeInShares = _claims[tile.box].makeClaim(userName);
        _users[userName].addShares(changeInShares);

        foreach (string user in _claims[tile.box].getOwners().Keys)
        {
            if (user != userName)
            {
                _users[user].addShares(changeInShares * -1);
            }
        }

        return tile.box;
    }
    public Dictionary<Geo.Box,double> getClaimsByUser(string userName)
    {
        Dictionary<Geo.Box, double> claims = new Dictionary<Geo.Box, double>();
        foreach(Geo.Box key in _claims.Keys)
        {
            double d = _claims[key].getFractionalOwnership(userName);
            if ( d > 0)
            {
                claims.Add(key, d);
            }
        }

        return claims;
    }
    public User getUser(string userName)
    {
        return _users[userName];
    }
    public Claim[] getClaims()
    {
        Claim[] c = new Claim[_claims.Count];
        _claims.Values.CopyTo(c,0);

        return c;
    }
    #region helperClasses
    public class Claim
    {
        private System.Collections.Generic.List<String> users;

        private Geo.Box _box = new Geo.Box();
        public Geo.Box box
        {
            get { return _box;  }
        }
        public Claim(double lat, double lng)
        {
            _box = Geo.getBox(lat, lng, 10);
            users = new List<string>(10);
        }

        /// <summary>
        /// register a claim for user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>change in number of frac shares</returns>
        public double makeClaim(string user)
        {
            if (users.Count >= 10)
            {
                users.RemoveAt(0);
            }

            users.Add(user);

            return 1.0/(Double)users.Count;
        }

        public Dictionary<string,double> getOwners()
        {
            Dictionary<string, double> userDic = new Dictionary<string, double>();

            foreach (string u in users)
            {
                if (!userDic.ContainsKey(u))
                {
                    userDic.Add(u, getFractionalOwnership(u));
                }
            }

            return userDic;
        }
        public double getFractionalOwnership(string user)
        {
            int count = 0;
            foreach (string u in users)
            {
                if (u == user)
                {
                    count++;
                }
            }

            return (Double)count / (Double)users.Count;
        }
    }
    public class User
    {
        public DateTime _checkPoint;
        private string _userName;
        private double _shares;
        private double _accountBalance;

        public User(string userName) 
        {
            _checkPoint = DateTime.Now;
            _userName = userName;
            _shares = 0;
        }

        public void addShares(double share)
        {
            getAccountBalance();
            _shares += share;
        }

        public double shares { get { return _shares; } }
        public double getAccountBalance()
        {
            TimeSpan ts = DateTime.Now - _checkPoint; _checkPoint = DateTime.Now;
            return _accountBalance += ts.TotalHours * _shares;
        }
    }

    #endregion
}