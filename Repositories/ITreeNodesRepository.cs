using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZadanieRekrutacyjneIdeo.Models;

namespace ZadanieRekrutacyjneIdeo.Repositories
{
    public interface ITreeNodesRepository
    {
        Task<List<TreeNode>> GetAllNodes();
        Task<List<TreeNode>> GetNodesWithout(int? x);
        Task AddTreeNode(TreeNode node);
        Task<TreeNode> GetTreeNodeByIdAsync(int? id);
        Task EditTreeNode(TreeNode node);
        bool TreeNodeExists(int id);
        Task RemoveAllNodes();
        Task RemoveTreeNodeAsync(TreeNode node);
    }
}
