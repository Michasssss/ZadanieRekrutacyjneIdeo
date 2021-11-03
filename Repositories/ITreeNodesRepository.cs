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
        Task<List<TreeNode>> GetNodesWithout(int? id);
        Task<List<TreeNode>> GetChildren(int Id);
        Task GetAllChildrens(int Id, List<TreeNode> nodes);
        Task<TreeNode> GetTreeNodeByIdAsync(int? id);
        Task AddTreeNode(TreeNode node);
        bool TreeNodeExists(int id);
        Task RemoveAllNodes();
        Task RemoveTreeNodeAsync(TreeNode node);
        Task RemoveWithChildrens(TreeNode node);
        Task ChangeParent(int idNode, int idNewNode, string name);
        Task LoadData();
    }
}
