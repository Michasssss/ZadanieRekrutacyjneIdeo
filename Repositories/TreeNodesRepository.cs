using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZadanieRekrutacyjneIdeo.Data;
using ZadanieRekrutacyjneIdeo.Models;

namespace ZadanieRekrutacyjneIdeo.Repositories
{
    public class TreeNodesRepository : ITreeNodesRepository
    {
        private readonly ApplicationDbContext _context;

        public TreeNodesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all nodes
        /// </summary>
        public async Task<List<TreeNode>> GetAllNodes()
        {
            var applicationDbContext = _context.TreeNodes.Include(t => t.Childs);
            return await applicationDbContext.ToListAsync();
        }

        /// <summary>
        /// Get list node without given node id
        /// </summary>
        /// <param name="id"></param>
        public async Task<List<TreeNode>> GetNodesWithout(int? id)
        {
            var applicationDbContext = _context.TreeNodes.Include(t => t.Parent).Where(n => n.ID != id);
            return await applicationDbContext.ToListAsync();
        }

        /// <summary>
        /// Get first childrens with the given node id
        /// </summary>
        /// <param name="Id"></param>
        public async Task<List<TreeNode>> GetChildren(int Id)
        {
            return await _context.TreeNodes.Include(t => t.Parent).Where(nd => nd.PID == Id).ToListAsync();
        }

        /// <summary>
        /// Get all childrens with the given node id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="nodes"></param>
        public async Task GetAllChildrens(int Id, List<TreeNode> nodes)
        {
            List<TreeNode> listChildrens = await GetChildren(Id);
            if (listChildrens.Count != 0)
            {
                nodes.AddRange(listChildrens);
                foreach (var item in listChildrens)
                {
                    await GetAllChildrens(item.ID, nodes);
                }
            }
        }

        /// <summary>
        /// Gets an element with the given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>node</returns>
        public async Task<TreeNode> GetTreeNodeByIdAsync(int? id)
        {
            var node = await _context.TreeNodes.Include(t => t.Parent).FirstOrDefaultAsync(n => n.ID == id);
            return node;
        }

        /// <summary>
        /// Add new node in database
        /// </summary>
        /// <param name="node"></param>
        public async Task AddTreeNode(TreeNode node)
        {
            await _context.TreeNodes.AddAsync(node);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Remove all nodes
        /// </summary>
        public async Task RemoveAllNodes()
        {
            _context.TreeNodes.RemoveRange(_context.TreeNodes);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Checks if the element with the given identifier exists
        /// </summary>
        /// <param name="id"></param>
        public bool TreeNodeExists(int id)
        {
            return _context.TreeNodes.Any(e => e.ID == id);
        }

        /// <summary>
        /// Remove node
        /// </summary>
        /// <param name="node"></param>
        public async Task RemoveTreeNodeAsync(TreeNode node)
        {
            _context.TreeNodes.Remove(node);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Remove node with childrens
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public async Task RemoveWithChildrens(TreeNode node)
        {
            var result = new List<TreeNode>() { node };
            await GetAllChildrens(node.ID, result);
            _context.TreeNodes.RemoveRange(result);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// This method edits a node by changing its name and parent
        /// </summary>
        /// <param name="idNode"></param>
        /// <param name="idNewNode"></param>
        /// <param name="name"></param>
        public async Task ChangeParent(int idNode, int idNewNode, string name)
        {
            TreeNode node = await GetTreeNodeByIdAsync(idNode);
            if (node.PID != null)
            {
                List<TreeNode> childrens = await GetChildren(idNode);
                var allChildrens = new List<TreeNode>();
                await GetAllChildrens(idNode, allChildrens);
                foreach (var item in allChildrens)
                {
                    if (item.ID == idNewNode)
                    {
                        foreach (var x in childrens)
                        {
                            x.PID = node.PID;
                        }
                    }
                }
                node.PID = idNewNode;
                node.Name = name;
                _context.TreeNodes.Update(node);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Removes data and adds new sample data
        /// </summary>
        public async Task LoadData()
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
            $"DBCC CHECKIDENT(TreeNodes, RESEED, 0); SET IDENTITY_INSERT TreeNodes ON; INSERT INTO TreeNodes(Id, Name, PID)VALUES(1, 'root', null),(2, 'Windows', 1),(3, 'Linux', 1),(4, 'MacOS', 3),(5, 'Office', 2),(6, 'InternetExplorer', 2),(7, 'Iceweasel', 3),(8, 'Word', 5),(9, 'Excel', 5),(10, 'PowerPoint', 5);");
        }

        public async Task<List<TreeNode>> GetAllSortedByName()
        {
            List<TreeNode> list =  _context.TreeNodes.Include(t => t.Childs).AsEnumerable().Where(t=> t.PID == null).ToList();
            await sortChildren(list.FirstOrDefault());
            return list;
        }

        async Task sortChildren(TreeNode node)
        {
            node.Childs = node.Childs.OrderBy(n => n.Name).ToList();
            foreach (var child in node.Childs)
            {
                await sortChildren(child);
            }
        }


    }
}
