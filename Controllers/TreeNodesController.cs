using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZadanieRekrutacyjneIdeo.Models;
using ZadanieRekrutacyjneIdeo.Repositories;

namespace ZadanieRekrutacyjneIdeo.Controllers
{
    public class TreeNodesController : Controller
    {
        private readonly ITreeNodesRepository _treeNodesRepository;

        public TreeNodesController(ITreeNodesRepository treeNodesRepository)
        {
            _treeNodesRepository = treeNodesRepository;
        }

        /// <summary>
        /// Removes the actual data from the database and adds sample data
        /// </summary>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> LoadDb()
        {
            await _treeNodesRepository.RemoveAllNodes();
            await _treeNodesRepository.LoadData();
            return RedirectToAction(nameof(Index));
        }
        /// <summary>
        /// This method is responsible for changing the index view
        /// </summary>
        /// <returns>Index View with list all nodes</returns>
        // GET: TreeNodes
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _treeNodesRepository.GetAllNodes());
        }

        /// <summary>
        /// This method is responsible for display details view with children and parent node 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Details View with the given id</returns>
        //GET: TreeNodes/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var treeNode = await _treeNodesRepository.GetTreeNodeByIdAsync(id);
            if (treeNode == null) return NotFound();
            ViewBag.Children = _treeNodesRepository.GetChildren(id.Value).Result;
            var result = new List<TreeNode>();
            await _treeNodesRepository.GetAllChildrens((int)id, result);
            return View(treeNode);
        }

        /// <summary>
        /// Create new node and add SelectList
        /// </summary>
        /// <returns>Create View</returns>
        // GET: TreeNodes/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            List<TreeNode> nodes = await _treeNodesRepository.GetAllNodes();
            ViewData["PID"] = new SelectList(nodes.OrderBy(n => n.Name), "ID", "Name");
            return View();
        }

        /// <summary>
        /// If element is validated creatern new node and redirect to index else Created View.
        /// </summary>
        /// <param name="treeNode"></param>
        /// <returns>If element is validated Index view, if not Create view</returns>
        // POST: TreeNodes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ID,Name,PID")] TreeNode treeNode)
        {
            if (ModelState.IsValid)
            {
                await _treeNodesRepository.AddTreeNode(treeNode);
                return RedirectToAction(nameof(Index));
            }
            ViewData["PID"] = new SelectList(await _treeNodesRepository.GetAllNodes(), "ID", "Name", treeNode.PID);
            return View(treeNode);
        }

        /// <summary>
        /// This method is responsible for initializing the parent change list and returning it in the view
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Edit View</returns>
        // GET: TreeNodes/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var nodes = await _treeNodesRepository.GetAllNodes();
            ViewBag.Nodes = nodes;
            var treeNode = await _treeNodesRepository.GetTreeNodeByIdAsync(id);
            if (treeNode == null) return NotFound();
            ViewData["PID"] = new SelectList(await _treeNodesRepository.GetNodesWithout(id), "ID", "Name", treeNode.PID);
            return View(treeNode);
        }

        /// <summary>
        /// This method is responsible for moving the node or renaming it
        /// </summary>
        /// <param name="id"></param>
        /// <param name="treeNode"></param>
        /// <returns>If element is validated Index view, if not Edit view</returns>
        // POST: TreeNodes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,PID")] TreeNode treeNode)
        {
            if (id != treeNode.ID) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    await _treeNodesRepository.ChangeParent(id, (int)treeNode.PID, treeNode.Name);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_treeNodesRepository.TreeNodeExists(treeNode.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PID"] = new SelectList(await _treeNodesRepository.GetNodesWithout(id), "ID", "Name", treeNode.PID);
            return View(treeNode);
        }
        /// <summary>
        /// This method removes all nodes in db
        /// </summary>
        /// <returns>Redirect to Index View</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveAllTreeNodes()
        {
            await _treeNodesRepository.RemoveAllNodes();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// This method prepares to removes nodes from the given id
        /// </summary>
        /// <param name="id"></param>
        // GET: TreeNodes/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var treeNode = await _treeNodesRepository.GetTreeNodeByIdAsync(id);
            var result = new List<TreeNode>();
            await _treeNodesRepository.GetAllChildrens((int)id, result);
            ViewBag.Children = result;
            ViewData["isLeaf"] = true;
            if (result.Count != 0)
            {
                ViewData["isLeaf"] = false;
            }
            if (treeNode == null) return NotFound();

            return View(treeNode);
        }

        /// <summary>
        /// This method is responsible for confirming the remove of the node
        /// </summary>
        /// <param name="id"></param>
        // POST: TreeNodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var treeNode = await _treeNodesRepository.GetTreeNodeByIdAsync(id);
            await _treeNodesRepository.RemoveTreeNodeAsync(treeNode);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// This method is responsible for confirming the remove of the node with childrens
        /// </summary>
        /// <param name="id"></param>
        [HttpPost, ActionName("DeleteWithChildrens")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteNodeWithChildrens(int id)
        {
            var treeNode = await _treeNodesRepository.GetTreeNodeByIdAsync(id);
            await _treeNodesRepository.RemoveWithChildrens(treeNode);
            return RedirectToAction(nameof(Index));
        }
    }
}
