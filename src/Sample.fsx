module Tree =
    type Tree = Empty | Branch of int * Tree * Tree
    let treeType = nameof Tree + "Type"
    let numType = 42

    let empty = Empty
    let singleton v = Branch(v, empty, empty)

    let rec add newitem = function 
    | Empty -> singleton newitem
    | Branch(num, _, _) as branch when num = newitem -> branch
    | Branch(num, left, right) as branch -> 
        if num <= newitem then Branch(num, add newitem left, right) else Branch(num, left, add newitem right) 

    let rec inorder = function
    | Empty -> []
    | Branch(num, left, right) -> (inorder left) @ [num] @ (inorder right)

