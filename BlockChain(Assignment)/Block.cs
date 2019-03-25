using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BlockChainAssignment
{
    public class Block : INotifyPropertyChanged
    {
        private const string signKey = "0000";

        #region Content Variables
        private string id;
        public string ID
        {
            get => id;
            set { id = value; NotifyPropertyChanged(); }
        }

        private string nonce;
        public string Nonce
        {
            get => nonce ;
            set { nonce = value; NotifyPropertyChanged(); }
        }

        private string data;
        public string Data
        {
            get => data;
            set { data = value; NotifyPropertyChanged(); }
        }

        private string prevHash;
        public string PrevHash
        {
            get => prevHash;
            set { prevHash = value; NotifyPropertyChanged(); }
        }

        private string hash;
        public string Hash
        {
            get => hash;
            set { hash = value; NotifyPropertyChanged(); }
        }

        // Variable Constructor
        public Block(string ID)
        {
            this.ID = ID;
            Nonce = "0";
            Data = string.Empty;
            PrevHash = "0000000000000";
            Hash = "";
            PropertyChanged += internalHandler;
        }
        #endregion

        #region Property Updaters
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void internalHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Hash" || e.PropertyName == "IsSigned") return;
            this.ReHash();
        }
        #endregion

        /// <summary>
        /// This is the method that recreates the Hash when a value is changed.
        /// </summary>
        /// <remarks> This is called by the internalHandler.  </remarks>
        /// <seealso cref="internalHandler(object sender, PropertyChangedEventArgs e)"/>
        public void ReHash()
        {
            Hash = HashConverter.HashGen(ID, Nonce, Data, PrevHash);
            PropertyChanged(this, new PropertyChangedEventArgs("IsSigned"));
        }

        /// <summary>
        /// This is the method to mine a nonce that returns a signed Hash.
        /// </summary>
        public void Mine()
        {
            if (Block.IsMining) return;
            Block.IsMining = true;
            Nonce = "0";

            while (!IsSigned)
                Nonce = (int.Parse(Nonce) + 1).ToString();

            IsMining = false;
            NotifyPropertyChanged("CurHash");
        }

        /// <summary>
        /// This checks if the hash is signed.
        /// A hash is signed if it's first 4 characters match the signKey.
        /// </summary>
        /// <remarks> The amount of characters that have to match the signKey is determined by the length of the signKey. </remarks>
        /// <seealso cref="signKey"/>
        public bool IsSigned => string.Equals(Hash.Substring(0, signKey.Length), signKey);

        /// <summary>
        /// This is used to prevent other blocks from mining if a block is already mining.
        /// </summary>
        /// <seealso cref="Mine()"/>
        public static bool IsMining { get; set; } = false;
    }

    class BlockList
    {
        /// <summary>
        /// This line creates an observable collection, a list, of type block.
        /// </summary>
        /// <seealso cref="Content Variables"/>
        public ObservableCollection<Block> BlockChain { get; } = new ObservableCollection<Block>();

        /// <summary>
        /// This code, when called, decides what to set the previous hash as. 
        /// It does this by asking if the current Block is Block 0, if this comes back as true
        /// the previous hash is the left option, false is the right option.
        /// </summary>
        /// <param name="B"> This is an instance of the type block. </remarks>
        public void Add(Block B)
        {
            B.PrevHash = B.ID.Equals("0") ? "0000000000000000000000000000000000000000" : BlockChain[BlockChain.Count - 1].Hash;
            BlockChain.Add(B);
            B.PropertyChanged += InternalHandler;
        }

        private void InternalHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Hash" ) return;
            if (sender is Block B)
            {
                if (Block.IsMining) return;

                int index = int.Parse(B.ID);
                if (index + 1 == BlockChain.Count) return;
                BlockChain[index + 1].PrevHash = BlockChain[index].Hash;
            }
        }

        /// <summary>
        /// This code generates the 5 Blocks.
        /// </summary>
        /// <returns>
        /// A list of 5 unique instanses of Block.
        /// </returns>
        public static BlockList GetInitializedBlockChain()
        {
            BlockList result = new BlockList();
            foreach (int i in Enumerable.Range(0, 5))
                result.Add(new Block(i.ToString()));
            return result;
        }
    }
}
