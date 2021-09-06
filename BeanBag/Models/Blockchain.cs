using System;
using System.Collections.Generic;

namespace BeanBag.Models
{
    public class Blockchain
    {
        public IList<Block> Chain { set;  get; }  
  
        public Blockchain()  
        {  
            InitializeChain();  
            AddGenesisBlock();  
        }
        
        public void InitializeChain()  
        {  
            Chain = new List<Block>();  
        }  
  
        public Block CreateGenesisBlock()  
        {  
            return new Block(DateTime.Now, null, "{}");  
        }

        public void AddGenesisBlock()
        {
            Chain.Add((CreateGenesisBlock()));
        }
      
        public Block GetLatestBlock()  
        {  
            return Chain[Chain.Count - 1];  
        }  
  
        public void AddBlock(Block block)  
        {  
            Block latestBlock = GetLatestBlock();  
            block.Index = latestBlock.Index + 1;  
            block.PreviousHash = latestBlock.Hash;  
            block.Hash = block.CalculateHash();  
            Chain.Add(block);  
        }  
    }
}