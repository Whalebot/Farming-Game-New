public class AreaChange : Interactable
{
    public int sceneID;
    public int spawnPointID;

    // Update is called once per frame
    public override void Interact()
    {
        StartPosition.spawnPointID = spawnPointID;
        TransitionManager.Instance.LoadScene(sceneID);
    }
}
